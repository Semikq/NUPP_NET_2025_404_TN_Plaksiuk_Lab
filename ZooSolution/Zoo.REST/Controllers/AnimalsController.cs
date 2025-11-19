using Microsoft.AspNetCore.Mvc;
using Zoo.REST.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly ICrudServiceAsync<Animal> _animalService;
    public AnimalsController(ICrudServiceAsync<Animal> animalService)
    {
        _animalService = animalService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AnimalModel>), 200)]
    public async Task<ActionResult<IEnumerable<AnimalModel>>> Get()
    {
        var animals = await _animalService.ReadAllAsync();

        var models = animals.Select(a => new AnimalModel
        {
            Id = a.Id,
            Species = a.Species,
            Name = a.Name,
            Age = a.Age,
            EnclosureId = a.EnclosureId
        });

        return Ok(models);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AnimalModel), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<AnimalModel>> Get(Guid id)
    {
        var animal = await _animalService.ReadAsync(id);

        if (animal == null)
        {
            return NotFound();
        }

        var model = new AnimalModel
        {
            Id = animal.Id,
            Species = animal.Species,
            Name = animal.Name,
            Age = animal.Age,
            EnclosureId = animal.EnclosureId
        };

        return Ok(model);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AnimalModel), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<AnimalModel>> Post([FromBody] AnimalCreateModel createModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newAnimal = new Animal
        {
            Species = createModel.Species,
            Name = createModel.Name,
            Age = createModel.Age
        };

        var success = await _animalService.CreateAsync(newAnimal);

        if (success)
        {
            var createdModel = new AnimalModel
            {
                Id = newAnimal.Id,
                Species = newAnimal.Species,
                Name = newAnimal.Name,
                Age = newAnimal.Age
            };

            return CreatedAtAction(nameof(Get), new { id = createdModel.Id }, createdModel);
        }

        return BadRequest("Помилка");
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Put(Guid id, [FromBody] AnimalModel updateModel)
    {
        if (id != updateModel.Id)
        {
            return BadRequest("Помилка ID");
        }

        var existingAnimal = await _animalService.ReadAsync(id);
        if (existingAnimal == null)
        {
            return NotFound();
        }

        existingAnimal.Species = updateModel.Species;
        existingAnimal.Name = updateModel.Name;
        existingAnimal.Age = updateModel.Age;
        existingAnimal.EnclosureId = updateModel.EnclosureId;

        var success = await _animalService.UpdateAsync(existingAnimal);
        await _animalService.SaveAsync();

        if (success)
        {
            return NoContent();
        }

        return StatusCode(500, "Помилка оновлення");
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var animalToRemove = await _animalService.ReadAsync(id);

        if (animalToRemove == null)
        {
            return NotFound();
        }

        var success = await _animalService.RemoveAsync(animalToRemove);
        await _animalService.SaveAsync();

        if (success)
        {
            return NoContent();
        }

        return StatusCode(500, "Помилка");
    }
}
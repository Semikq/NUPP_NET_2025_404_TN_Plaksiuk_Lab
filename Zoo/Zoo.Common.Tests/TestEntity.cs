using System;

namespace Zoo.Common.Tests
{
    /// <summary>
    /// </summary>
    public class TestEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }

        public TestEntity()
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
            Value = 0;
        }

        public TestEntity(string name, int value)
        {
            Id = Guid.NewGuid();
            Name = name;
            Value = value;
        }

        public TestEntity(Guid id, string name, int value)
        {
            Id = id;
            Name = name;
            Value = value;
        }
    }
}
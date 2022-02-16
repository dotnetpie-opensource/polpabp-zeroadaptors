namespace PolpAbp.ZeroAdaptors
{
    public class NameValueDto<T>
    {
        public NameValueDto() { }

        public NameValueDto(NameValueDto<T> nameValue)
        {
            Name = nameValue.Name;
            Value = nameValue.Value;
        }

        public NameValueDto(string name, T value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }
        public T Value { get; set; }
    }
}

namespace WpfClient
{   
    public interface IBuffer
    {
        uint Capacity { get; }
        uint Length { get; set; }
    }
}
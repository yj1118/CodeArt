

namespace CodeArt.DTO
{
    internal delegate void SerializeMethod(object instance, IDTOWriter writer);
    internal delegate void DeserializeMethod(object instance, IDTOReader reader);
}

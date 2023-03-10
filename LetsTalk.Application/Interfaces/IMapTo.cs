using AutoMapper;

namespace LetsTalk.Interfaces;

public interface IMapTo<T>
{
    void MapTo(Profile profile) => profile.CreateMap(GetType(), typeof(T));
}

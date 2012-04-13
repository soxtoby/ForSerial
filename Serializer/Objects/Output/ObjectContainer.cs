using System;

namespace ForSerial.Objects
{
    public interface ObjectContainer : ObjectOutput
    {
        void SetCurrentProperty(string name);
        ObjectContainer CreateStructure();
        ObjectContainer CreateStructure(string typeIdentifier);
        ObjectContainer CreateSequence();
        bool CanCreateValue(object value);
        void WriteValue(object value);
        void Add(ObjectOutput value);
        PreBuildInfo GetPreBuildInfo(Type readerType);
    }
}
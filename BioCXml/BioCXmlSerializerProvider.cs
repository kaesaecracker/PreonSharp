using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;

namespace BioCXml;

public static class BioCXmlSerializerProvider
{
    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Collection))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Annotation))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Document))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Infon))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Location))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Node))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Passage))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Relation))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(Sentence))]
    private static XmlSerializer Get()
    {
        return new XmlSerializer(typeof(Collection));
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
    public static Collection? Deserialize(XmlReader reader)
    {
        return (Collection?)Get().Deserialize(reader);
    }
}
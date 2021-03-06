namespace AgileObjects.AgileMapper.Members.Sources
{
    using ObjectPopulation;

    internal class ElementMembersSource : IMembersSource
    {
        public ElementMembersSource(IObjectMappingData parent)
        {
            Parent = parent;
        }

        public IObjectMappingData Parent { get; }

        public IQualifiedMember GetSourceMember<TSource, TTarget>()
        {
            var sourceElementMember = Parent.MapperData.SourceMember.GetElementMember();
            var targetElementMember = GetTargetMember<TSource, TTarget>();

            sourceElementMember = Parent.MapperData
                .MapperContext
                .QualifiedMemberFactory
                .GetFinalSourceMember(sourceElementMember, targetElementMember);

            return sourceElementMember;
        }

        public QualifiedMember GetTargetMember<TSource, TTarget>()
            => Parent.MapperData.TargetMember.GetElementMember();
    }
}
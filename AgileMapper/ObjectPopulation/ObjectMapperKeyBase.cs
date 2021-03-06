namespace AgileObjects.AgileMapper.ObjectPopulation
{
    using System;
    using Members;
    using Members.Sources;

    internal abstract class ObjectMapperKeyBase
    {
        private Func<IMappingData, bool> _sourceMemberTypeTester;

        protected ObjectMapperKeyBase(MappingTypes mappingTypes)
        {
            MappingTypes = mappingTypes;
        }

        public MappingTypes MappingTypes { get; }

        public IObjectMappingData MappingData { get; set; }

        protected bool TypesMatch(ObjectMapperKeyBase otherKey) => otherKey.MappingTypes.Equals(MappingTypes);

        public void AddSourceMemberTypeTester(Func<IMappingData, bool> tester)
            => _sourceMemberTypeTester = tester;

        protected bool SourceHasRequiredTypes(ObjectMapperKeyBase otherKey)
            => (_sourceMemberTypeTester == null) || _sourceMemberTypeTester.Invoke(otherKey.MappingData);

        public abstract IMembersSource GetMembersSource(IObjectMappingData parentMappingData);

        public ObjectMapperKeyBase WithTypes<TNewSource, TNewTarget>()
        {
            if (MappingTypes.RuntimeTypesAreTheSame)
            {
                return this;
            }

            return CreateInstance(MappingTypes.WithTypes<TNewSource, TNewTarget>());
        }

        protected abstract ObjectMapperKeyBase CreateInstance(MappingTypes newMappingTypes);
    }
}
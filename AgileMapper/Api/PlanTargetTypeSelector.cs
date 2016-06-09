﻿namespace AgileObjects.AgileMapper.Api
{
    using Plans;

    public class PlanTargetTypeSelector<TSource>
    {
        private readonly MapperContext _mapperContext;

        internal PlanTargetTypeSelector(MapperContext mapperContext)
        {
            _mapperContext = mapperContext;
        }

        public string ToANew<TResult>() where TResult : class
        {
            using (var mappingContext = new MappingContext(_mapperContext.RuleSets.CreateNew, _mapperContext))
            {
                return MappingPlan.For<TSource, TResult>(mappingContext);
            }
        }
    }
}
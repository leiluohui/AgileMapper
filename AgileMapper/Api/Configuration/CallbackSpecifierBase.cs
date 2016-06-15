﻿namespace AgileObjects.AgileMapper.Api.Configuration
{
    using ObjectPopulation;

    internal abstract class CallbackSpecifierBase
    {
        protected CallbackSpecifierBase(CallbackPosition callbackPosition, MapperContext mapperContext)
            : this(callbackPosition, MappingConfigInfo.AllRuleSetsAndSourceTypes(mapperContext))
        {
        }

        protected CallbackSpecifierBase(CallbackPosition callbackPosition, MappingConfigInfo configInfo)
        {
            CallbackPosition = callbackPosition;
            ConfigInfo = configInfo;
        }

        protected CallbackPosition CallbackPosition { get; }

        protected MappingConfigInfo ConfigInfo { get; }
    }
}
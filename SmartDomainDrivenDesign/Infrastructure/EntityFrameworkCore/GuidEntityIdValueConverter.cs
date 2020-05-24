using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartDomainDrivenDesign.Domain.Abstract;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SmartDomainDrivenDesign.Infrastructure.EntityFrameworkCore
{
    internal class GuidEntityIdValueConverter<TIdentifier> : ValueConverter<TIdentifier, Guid> where TIdentifier : GuidEntityId
    {
        private static readonly Func<Guid, TIdentifier> constructor;

        /// <summary>
        /// This method compiles a Func using IL emitting to avoid calling Activator.CreateInstance and reflection every time.
        /// </summary>
        static GuidEntityIdValueConverter()
        {
            Type identifier = typeof(TIdentifier);
            Type[] args = new Type[] { typeof(Guid) };
            ConstructorInfo constructorInfo = identifier.GetConstructor(args);

            DynamicMethod dynamicMethod = new DynamicMethod("DM$_" + identifier.Name, identifier, args, identifier);
            ILGenerator ilGen = dynamicMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg, 0);
            ilGen.Emit(OpCodes.Newobj, constructorInfo);
            ilGen.Emit(OpCodes.Ret);

            constructor = (Func<Guid, TIdentifier>)dynamicMethod.CreateDelegate(typeof(Func<Guid, TIdentifier>));
        }

        public GuidEntityIdValueConverter() : base(id => id.Id, value => constructor(value)) { }
    }
}

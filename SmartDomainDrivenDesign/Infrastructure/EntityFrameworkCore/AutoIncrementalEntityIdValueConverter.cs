using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartDomainDrivenDesign.Domain.Abstract;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SmartDomainDrivenDesign.Infrastructure.EntityFrameworkCore
{
    internal class AutoIncrementalEntityIdValueConverter<TIdentifier> : ValueConverter<TIdentifier, long> where TIdentifier : AutoIncrementalEntityId
    {
        private static readonly Func<long, TIdentifier> constructor;

        /// <summary>
        /// This method compiles a Func using IL emitting to avoid calling Activator.CreateInstance and reflection every time.
        /// </summary>
        static AutoIncrementalEntityIdValueConverter()
        {
            Type identifier = typeof(TIdentifier);
            Type[] args = new Type[] { typeof(long) };
            ConstructorInfo constructorInfo = identifier.GetConstructor(args);

            DynamicMethod dynamicMethod = new DynamicMethod("DM$_" + identifier.Name, identifier, args, identifier);
            ILGenerator ilGen = dynamicMethod.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg, 0);
            ilGen.Emit(OpCodes.Newobj, constructorInfo);
            ilGen.Emit(OpCodes.Ret);

            constructor = (Func<long, TIdentifier>)dynamicMethod.CreateDelegate(typeof(Func<long, TIdentifier>));
        }

        public AutoIncrementalEntityIdValueConverter() : base(id => id.Id, value => constructor(value)) { }
    }
}

using System.Collections.Generic;
using Code.Core.Abstractions;

namespace Code.Infrastructure.Services.SaveLoad.Registry
{
   public sealed class SaveLoadRegistry : ISaveLoadRegistry
   {
      private readonly IReadOnlyList<ISaveLoad> _saveables;

      public SaveLoadRegistry(IReadOnlyList<ISaveLoad> saveables)
      {
         _saveables = saveables;
      }

      public IEnumerable<ISaveLoad> GetSaveLoadServices() => _saveables;
   }
}

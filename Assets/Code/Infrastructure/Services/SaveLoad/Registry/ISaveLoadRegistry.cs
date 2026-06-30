using System.Collections.Generic;
using Code.Core.Abstractions;

namespace Code.Infrastructure.Services.SaveLoad.Registry
{
   // Read-only view over every registered ISaveLoad. The collection is populated
   // by the DI container (VContainer resolves all ISaveLoad implementations), so
   // saveables never register themselves manually.
   public interface ISaveLoadRegistry
   {
      IEnumerable<ISaveLoad> GetSaveLoadServices();
   }
}

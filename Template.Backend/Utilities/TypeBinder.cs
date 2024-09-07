using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace Template.Backend.Utilities
{
    public class TypeBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var propertyName = bindingContext.ModelName;
            var value = bindingContext.ValueProvider.GetValue(propertyName);

            if (value == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            try
            {
                var typeDestination = bindingContext.ModelMetadata.ModelType;
                var deserializedValue = JsonSerializer.Deserialize(value.FirstValue!,
                    typeDestination, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                bindingContext.Result = ModelBindingResult.Success(deserializedValue);
            }
            catch
            {
                bindingContext.ModelState.TryAddModelError(propertyName, "El valor dado no es del tipo adecuado");
            }

            return Task.CompletedTask;
        }
    }
}

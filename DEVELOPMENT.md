# Development Guidelines

This document provides guidelines for developing ArcGIS Pro Add-ins and testing GitHub Copilot capabilities.

## ArcGIS Pro Add-in Architecture

### Module Pattern

ArcGIS Pro Add-ins use a module-based architecture:
- Each add-in has one or more modules defined in Config.daml
- Modules contain controls (buttons, tools, dockpanes, etc.)
- The main module class manages initialization and lifecycle

### Control Types

#### Buttons
- Simple click handlers
- Use for actions that don't require map interaction
- Examples: Open dialogs, run geoprocessing tools, export data

#### Tools
- Interactive map tools
- Handle mouse events on the map
- Examples: Selection tools, sketch tools, identify tools

#### Dockpanes
- Persistent UI panels
- Can be docked or floating
- Examples: TOC replacements, custom property viewers

#### Panes
- Document-style windows
- Can be tabbed with map views
- Examples: Custom editors, reports

### MVVM Pattern

ArcGIS Pro Add-ins use the Model-View-ViewModel (MVVM) pattern:
- **View**: XAML files defining UI
- **ViewModel**: C# classes with business logic
- **Model**: Data classes

## Copilot Testing Scenarios

### 1. API Discovery

Test Copilot's ability to suggest ArcGIS Pro SDK APIs:

```csharp
// Try typing: "Get the active map view"
// Expected: Copilot suggests MapView.Active

// Try typing: "Get all layers in the map"
// Expected: Copilot suggests code to iterate map layers
```

### 2. Pattern Completion

Test common add-in patterns:

```csharp
// Try typing: "QueuedTask to access"
// Expected: Copilot suggests QueuedTask.Run() pattern

// Try typing: "Check if map view exists"
// Expected: Copilot suggests MapView.Active != null
```

### 3. Error Handling

Test exception handling suggestions:

```csharp
// Try typing: "try catch for accessing"
// Expected: Copilot suggests try-catch with appropriate exceptions
```

### 4. DAML Completion

Test Config.daml editing:

```xml
<!-- Try adding: <button -->
<!-- Expected: Copilot suggests button attributes -->

<!-- Try adding: <dockpane -->
<!-- Expected: Copilot suggests dockpane structure -->
```

### 5. Documentation Generation

Test XML documentation:

```csharp
/// <summary>
/// Try typing: Gets or sets
/// Expected: Copilot completes documentation
```

## Best Practices

### Code Organization

1. **Separate Concerns**: Keep UI logic in view-models, business logic in separate classes
2. **Async Patterns**: Use QueuedTask for map operations, async/await for I/O
3. **Resource Management**: Properly dispose of resources (cursors, selections, etc.)
4. **Error Handling**: Always handle exceptions, especially in map operations

### Config.daml Guidelines

1. **Unique IDs**: Use consistent naming convention (AddInName_ComponentName)
2. **Keytips**: Provide keyboard shortcuts for accessibility
3. **Tooltips**: Add helpful descriptions for all controls
4. **Images**: Use consistent icon sizes (16x16 for small, 32x32 for large)

### Performance Tips

1. **QueuedTask**: Use for all map/layer operations
2. **Async Methods**: Don't block UI thread
3. **Caching**: Cache frequently accessed data
4. **Lazy Loading**: Initialize resources only when needed

## Common Patterns

### Accessing Active Map

```csharp
var mapView = MapView.Active;
if (mapView == null)
    return;

await QueuedTask.Run(() =>
{
    var map = mapView.Map;
    // Work with map
});
```

### Working with Layers

```csharp
await QueuedTask.Run(() =>
{
    var layers = MapView.Active.Map.GetLayersAsFlattenedList();
    foreach (var layer in layers)
    {
        // Process layer
    }
});
```

### Showing Messages

```csharp
// Simple message
MessageBox.Show("Message", "Title");

// Or use ArcGIS Pro notification
ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Message");
```

### Executing Geoprocessing

```csharp
var parameters = Geoprocessing.MakeValueArray(inputParam1, inputParam2);
var result = await Geoprocessing.ExecuteToolAsync("ToolName", parameters);
```

## Testing Checklist

When testing with Copilot, evaluate:

- [ ] Code completion accuracy
- [ ] API suggestion relevance
- [ ] Pattern recognition
- [ ] Error handling suggestions
- [ ] Documentation quality
- [ ] DAML completion
- [ ] Async/await patterns
- [ ] Resource disposal patterns
- [ ] Exception handling
- [ ] Threading model understanding (QueuedTask)

## Debugging Tips

1. **Attach to Process**: Attach debugger to ArcGISPro.exe
2. **Breakpoints**: Set breakpoints in button click handlers
3. **Logging**: Use Debug.WriteLine or log to file
4. **Pro SDK Diagnostics**: Enable verbose logging in ArcGIS Pro

## Additional Resources

### Official Documentation
- [Pro SDK Guide](https://github.com/Esri/arcgis-pro-sdk/wiki)
- [Pro SDK API Reference](https://pro.arcgis.com/en/pro-app/latest/sdk/api-reference/)

### Community Resources
- [GeoNet ArcGIS Pro SDK Forum](https://community.esri.com/t5/arcgis-pro-sdk-questions/bd-p/arcgis-pro-sdk-questions)
- [GitHub - Pro SDK Samples](https://github.com/Esri/arcgis-pro-sdk-community-samples)

### Video Tutorials
- [Esri Events - SDK Sessions](https://www.esri.com/videos)
- [YouTube - ArcGIS Pro SDK](https://www.youtube.com/results?search_query=arcgis+pro+sdk)

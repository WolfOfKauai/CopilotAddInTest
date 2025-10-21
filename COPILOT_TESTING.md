# GitHub Copilot Testing Scenarios for ArcGIS Pro Add-ins

This document provides specific scenarios to test GitHub Copilot's effectiveness for ArcGIS Pro Add-in development.

## Test Scenario 1: Basic Button Implementation

**Objective**: Test Copilot's ability to suggest standard button patterns

**Steps**:
1. Open `Button1.cs`
2. Position cursor in the `OnClick()` method
3. Start typing: `// Get the active map view`
4. Observe if Copilot suggests: `var mapView = MapView.Active;`

**Expected Result**: Copilot should suggest common ArcGIS Pro SDK patterns for accessing the active map.

## Test Scenario 2: QueuedTask Pattern

**Objective**: Test understanding of ArcGIS Pro threading model

**Steps**:
1. Create a new method in any class
2. Type: `// Run on the MCT thread`
3. Observe if Copilot suggests the QueuedTask.Run pattern

**Expected Result**: 
```csharp
await QueuedTask.Run(() =>
{
    // code here
});
```

## Test Scenario 3: Layer Iteration

**Objective**: Test API knowledge for working with layers

**Steps**:
1. In a new method, type: `// Get all layers from the map`
2. Continue typing and observe suggestions

**Expected Result**:
```csharp
var layers = MapView.Active?.Map?.GetLayersAsFlattenedList();
if (layers != null)
{
    foreach (var layer in layers)
    {
        // process layer
    }
}
```

## Test Scenario 4: DAML Completion

**Objective**: Test Config.daml XML completion

**Steps**:
1. Open `Config.daml`
2. Add a new line in the `<controls>` section
3. Type: `<button id="CopilotAddIn_NewButton"`
4. Observe attribute suggestions

**Expected Result**: Copilot suggests common button attributes like `caption`, `className`, `loadOnClick`, etc.

## Test Scenario 5: Error Handling

**Objective**: Test exception handling suggestions

**Steps**:
1. In `Button1.cs`, wrap map access code
2. Type: `try` and press Enter
3. Observe the suggested catch blocks

**Expected Result**: Copilot should suggest appropriate exception types and handling code.

## Test Scenario 6: Async Patterns

**Objective**: Test async/await pattern recognition

**Steps**:
1. Create a new async method
2. Type: `public async Task`
3. Continue and observe method signature suggestions

**Expected Result**: Proper async method patterns with Task return types.

## Test Scenario 7: Property Implementation

**Objective**: Test INotifyPropertyChanged pattern

**Steps**:
1. Open `Dockpane1.cs`
2. Start adding a new property
3. Type: `private string _newProperty;`
4. Press Enter and type: `public string NewProperty`

**Expected Result**: Copilot suggests full property implementation with getter, setter, and NotifyPropertyChanged call.

## Test Scenario 8: XAML Binding

**Objective**: Test XAML data binding suggestions

**Steps**:
1. Open `Dockpane1.xaml`
2. Add a new TextBlock
3. Type: `<TextBlock Text="{Binding`

**Expected Result**: Copilot suggests available properties from the view model.

## Test Scenario 9: Geoprocessing Tool Execution

**Objective**: Test knowledge of geoprocessing patterns

**Steps**:
1. Create a new method
2. Type: `// Execute the Buffer tool`
3. Continue typing and observe suggestions

**Expected Result**:
```csharp
var parameters = Geoprocessing.MakeValueArray(inputFeatures, outputFeatures, distance);
var result = await Geoprocessing.ExecuteToolAsync("Buffer_analysis", parameters);
```

## Test Scenario 10: Selection Handling

**Objective**: Test selection API patterns

**Steps**:
1. Type: `// Get selected features from the layer`
2. Observe suggestions for selection access

**Expected Result**: Copilot suggests patterns for accessing layer selection.

## Test Scenario 11: Map Tool Mouse Events

**Objective**: Test interactive tool patterns

**Steps**:
1. Open `MapTool1.cs`
2. In `OnToolMouseDown`, type: `// Get the clicked point`
3. Observe suggestions

**Expected Result**: Copilot suggests accessing map point from event arguments.

## Test Scenario 12: XML Documentation

**Objective**: Test documentation generation

**Steps**:
1. Add a new method
2. Type `///` above the method
3. Press Enter

**Expected Result**: Copilot generates XML documentation summary with parameter descriptions.

## Test Scenario 13: Command Implementation

**Objective**: Test RelayCommand pattern for MVVM

**Steps**:
1. In a view model, type: `private RelayCommand _myCommand;`
2. Continue with property implementation

**Expected Result**: Copilot suggests proper command property with getter and lazy initialization.

## Test Scenario 14: Resource Disposal

**Objective**: Test understanding of IDisposable pattern

**Steps**:
1. Create code that uses cursors or selections
2. Type: `using (var`
3. Observe suggestions

**Expected Result**: Copilot suggests using statement for proper resource disposal.

## Test Scenario 15: Symbology Access

**Objective**: Test renderer and symbol API knowledge

**Steps**:
1. Type: `// Get the layer's renderer`
2. Continue typing

**Expected Result**: Copilot suggests code to access and work with layer symbology.

## Evaluation Criteria

For each test scenario, evaluate Copilot on:

1. **Accuracy** (1-5): Does the suggestion match ArcGIS Pro SDK patterns?
2. **Relevance** (1-5): Is the suggestion appropriate for the context?
3. **Completeness** (1-5): Does the suggestion include necessary null checks, async patterns, etc.?
4. **Best Practices** (1-5): Does it follow ArcGIS Pro SDK best practices?

## Recording Results

Create a results table:

| Scenario | Accuracy | Relevance | Completeness | Best Practices | Notes |
|----------|----------|-----------|--------------|----------------|-------|
| 1        |          |           |              |                |       |
| 2        |          |           |              |                |       |
| ...      |          |           |              |                |       |

## Additional Test Ideas

- Testing with different ArcGIS Pro SDK versions
- Testing context-aware suggestions in different file types
- Testing refactoring suggestions
- Testing code explanation capabilities
- Testing bug detection and fixes
- Testing performance optimization suggestions

## Notes

- Some suggestions may vary based on Copilot model updates
- Context from surrounding code affects suggestion quality
- Testing should be repeated across different development sessions
- Document any particularly helpful or problematic suggestions

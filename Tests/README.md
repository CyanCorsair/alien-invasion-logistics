# Alien Invasion Logistics - Test Suite

This test project contains comprehensive unit tests for the Alien Invasion Logistics game.

## Test Framework

- **XUnit** - Primary testing framework
- **FluentAssertions** - Fluent assertion library for readable test assertions
- **Moq** - Mocking framework for dependencies
- **Microsoft.EntityFrameworkCore.InMemory** - In-memory database provider for testing EF Core

## Test Structure

```
Tests/
├── Database/               # Database and EF Core tests
│   ├── GameDataContextTests.cs
│   └── GameDataServiceTests.cs
├── GameObjects/           # Game object tests (Sun, Planet, etc.)
│   └── StellarBodyTests.cs
├── Models/                # Database model tests
│   └── DatabaseModelTests.cs
├── Utilities/             # Utility class tests
│   └── ErrorHandlerTests.cs
└── Helpers/               # Test helper classes
    ├── TestDbContextFactory.cs
    └── TestDataBuilder.cs
```

## Running Tests

### Command Line
```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Visual Studio
1. Open Test Explorer (Test → Test Explorer)
2. Click "Run All" to run all tests
3. Right-click individual tests to run or debug them

### Visual Studio Code
1. Install the .NET Core Test Explorer extension
2. Tests will appear in the Test Explorer sidebar
3. Click the play button to run tests

## Test Categories

### Database Tests
Tests for Entity Framework Core context and data service:
- Navigation property relationships
- Owned type configurations
- Many-to-many relationships
- Async database operations
- Data persistence and retrieval

### Game Object Tests
Tests for game entities:
- Sun and Planet initialization
- Property getters/setters
- IStellarBody interface compliance
- Position and velocity calculations

### Model Tests
Tests for database models:
- Model initialization
- Property storage
- Default values
- Navigation properties

### Utility Tests
Tests for helper and utility classes:
- Error handler logging
- Different error severity levels
- Exception handling

## Test Helpers

### TestDbContextFactory
Creates in-memory database contexts for testing:
```csharp
var context = TestDbContextFactory.CreateInMemoryContext();
```

### TestDataBuilder
Provides pre-configured test data:
```csharp
var gameSettings = TestDataBuilder.CreateDefaultGameSettings();
var playerState = TestDataBuilder.CreateTestPlayerState();
var gameData = TestDataBuilder.CreateTestGameData();
```

## Writing New Tests

### Basic Test Structure
```csharp
[Fact]
public void MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    var sut = new SystemUnderTest();

    // Act
    var result = sut.MethodToTest();

    // Assert
    result.Should().Be(expectedValue);
}
```

### Theory Tests (Parameterized)
```csharp
[Theory]
[InlineData(1, "expected1")]
[InlineData(2, "expected2")]
public void MethodName_WithDifferentInputs_ReturnsExpected(int input, string expected)
{
    // Test implementation
}
```

### Async Tests
```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedBehavior()
{
    // Arrange
    var sut = new SystemUnderTest();

    // Act
    var result = await sut.AsyncMethod();

    // Assert
    result.Should().NotBeNull();
}
```

## Best Practices

1. **Naming Convention**: `MethodName_Scenario_ExpectedBehavior`
2. **AAA Pattern**: Always use Arrange, Act, Assert sections
3. **One Assert Per Test**: Focus each test on a single behavior
4. **Use FluentAssertions**: For readable and maintainable assertions
5. **Dispose Resources**: Implement `IDisposable` for tests that use resources
6. **Test Independence**: Each test should be independent and not rely on test execution order
7. **Use In-Memory Database**: For fast, isolated database tests
8. **Mock External Dependencies**: Use Moq to mock services and dependencies

## Code Coverage

To generate a code coverage report:

```bash
dotnet test --collect:"XPlat Code Coverage"
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

Then open `coveragereport/index.html` to view the coverage report.

## Continuous Integration

Tests should be run on every commit/PR:
- All tests must pass before merging
- Maintain minimum 70% code coverage
- No skipped or ignored tests without justification

## Troubleshooting

### Tests Not Discovered
- Ensure test class is public
- Ensure test methods have `[Fact]` or `[Theory]` attribute
- Rebuild the solution

### In-Memory Database Issues
- Each test should use a unique database name
- Dispose contexts properly
- Don't share context instances between tests

### Async Test Deadlocks
- Use `await` instead of `.Result` or `.Wait()`
- Don't mix async and blocking calls

# ProtoValidate.NET Source Code Structure

This directory contains the core implementation of ProtoValidate.NET, a validation library for Protocol Buffers messages. Below is an explanation of the key files and their relationships.

## Core Files

### `IValidator.cs`
- Defines the core interface for message validation
- Contains two main methods:
  - `Validate`: Validates a Protocol Buffer message
  - `GetEvaluatorDebugString`: Provides debug information about the validation process

### `Validator.cs`
- Main implementation of the `IValidator` interface
- Handles the validation logic for Protocol Buffer messages
- Manages the evaluation of validation rules using CEL (Common Expression Language)
- Contains methods for:
  - Message validation
  - Debug string generation
  - Evaluator initialization and management

### `ValidationResult.cs`
- Represents the result of a validation operation
- Contains:
  - List of validation violations
  - Success/failure status
  - String representation of validation results
- Extends the base `Violation` class from Buf.Validate

### `ValidatorOptions.cs`
- Configuration options for the validator
- Controls behavior such as:
  - File descriptor loading
  - Lazy evaluation settings
  - Pre-loading of descriptors

### `ServiceCollectionExtensions.cs`
- Provides dependency injection extensions
- Integrates the validator with Microsoft's dependency injection system
- Makes it easy to register the validator in ASP.NET Core applications

### `Extensions.cs`
- Contains utility extension methods
- Provides helper functions for working with the validator

## Directory Structure

### `/Internal`
Contains internal implementation details and helper classes used by the main validator.

### `/Exceptions`
Contains custom exception types used throughout the library.

### `/proto`
Contains Protocol Buffer definition files used by the library.

### `/build`
Contains build-related files and scripts.

## Usage Flow

1. The `Validator` class is the main entry point for validation
2. It uses the `IValidator` interface to define its contract
3. Validation results are returned as `ValidationResult` objects
4. Configuration is handled through `ValidatorOptions`
5. The library can be integrated into applications using `ServiceCollectionExtensions`

## Dependencies

- Google.Protobuf: For Protocol Buffer message handling
- Cel: For Common Expression Language evaluation
- Microsoft.Extensions.Options: For configuration management 
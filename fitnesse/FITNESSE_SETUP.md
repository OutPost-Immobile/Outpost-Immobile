# FitNesse with FitSharp Setup Guide

## Overview
This guide explains how to set up and run FitNesse acceptance tests with FitSharp for the OutpostImmobile project.

## Prerequisites

1. **.NET 10 SDK** installed
2. **Java Runtime Environment (JRE)** - FitNesse requires Java to run
3. **FitNesse standalone JAR** - Download from [fitnesse.org](http://fitnesse.org/FitNesseDownload)

## Project Structure

```
fitnesse/
├── FitNesseRoot/
│   ├── OutpostImmobileAcceptance/     # Test suite
│   │   ├── SetUp/                      # Common setup for all tests
│   │   ├── PU07OdbiorPaczkiZMagazynu/  # PU07 test case
│   │   └── PU08ZawiezienieDoMagazynu/  # PU08 test case
│   └── content.txt                     # Root configuration
├── suite.local.config.xml              # Local FitSharp configuration
└── FITNESSE_SETUP.md                   # This file

Core/OutpostImmobile.Core.Tests.Acceptance/
├── FitNesse/
│   ├── Context/
│   │   └── TestContext.cs              # Shared test context with DI
│   └── Fixtures/
│       ├── BaseFixture.cs              # Base class for all fixtures
│       ├── SetupFixture.cs             # Test data setup
│       ├── ParcelFixture.cs            # Parcel commands/queries
│       ├── MaczkopatFixture.cs         # Maczkopat commands/queries
│       ├── AuthorizationFixture.cs     # Auth simulation
│       └── NotificationFixture.cs      # Email verification
├── suite.docker.config.xml             # Docker FitSharp configuration
└── OutpostImmobile.Core.Tests.Acceptance.csproj
```

## Step 1: Build the Test Project

```bash
cd /home/kollibroman/LosoweProjekty/Outpost-Immobile

# Build the acceptance test project
dotnet build Core/OutpostImmobile.Core.Tests.Acceptance/OutpostImmobile.Core.Tests.Acceptance.csproj -c Debug

# Publish for FitSharp (copies Runner.dll)
dotnet publish Core/OutpostImmobile.Core.Tests.Acceptance/OutpostImmobile.Core.Tests.Acceptance.csproj -c Debug -o Core/OutpostImmobile.Core.Tests.Acceptance/bin/Debug/net10.0/publish
```

## Step 2: Download FitNesse

```bash
cd fitnesse

# Download FitNesse standalone JAR (if not already present)
wget -O fitnesse-standalone.jar https://fitnesse.org/fitnesse-standalone.jar
```

## Step 3: Start FitNesse Server

### Option A: Local Development

```bash
cd /home/kollibroman/LosoweProjekty/Outpost-Immobile/fitnesse

# Start FitNesse on port 8080
java -jar fitnesse-standalone.jar -p 8080 -d . -e 0
```

Then open browser at: http://localhost:8080/OutpostImmobileAcceptance

### Option B: Using Docker (for CI/CD)

The project includes Docker configuration. See `docker-compose.yml` and `FitnesseDocker/` folder.

## Step 4: Run Tests

### Via FitNesse Web UI

1. Navigate to http://localhost:8080/OutpostImmobileAcceptance
2. Click "Suite" button to run all tests
3. Or navigate to individual test pages:
   - http://localhost:8080/OutpostImmobileAcceptance.PU07OdbiorPaczkiZMagazynu
   - http://localhost:8080/OutpostImmobileAcceptance.PU08ZawiezienieDoMagazynu
4. Click "Test" button on the page

### Via Command Line

```bash
cd /home/kollibroman/LosoweProjekty/Outpost-Immobile/fitnesse

# Run entire suite
java -jar fitnesse-standalone.jar -c "OutpostImmobileAcceptance?suite&format=text"

# Run single test
java -jar fitnesse-standalone.jar -c "OutpostImmobileAcceptance.PU07OdbiorPaczkiZMagazynu?test&format=text"
```

## Configuration Files

### suite.local.config.xml (Local Development)
```xml
<suiteConfig>
  <ApplicationUnderTest>
    <AddAssembly>../Core/OutpostImmobile.Core.Tests.Acceptance/bin/Debug/net10.0/OutpostImmobile.Core.Tests.Acceptance.dll</AddAssembly>
    <Namespace>OutpostImmobile.Core.Tests.Acceptance.FitNesse.Fixtures</Namespace>
  </ApplicationUnderTest>
  <Settings>
    <Runner>FitSharp.Slim.Service.Runner</Runner>
  </Settings>
</suiteConfig>
```

### FitNesseRoot/content.txt
```
!define TEST_SYSTEM {slim}
!define COMMAND_PATTERN {dotnet %m -c suite.local.config.xml %p}
!define TEST_RUNNER {../Core/OutpostImmobile.Core.Tests.Acceptance/bin/Debug/net10.0/Runner.dll}
```

## Fixture Overview

### SetupFixture
Creates test data (maczkopats, users, parcels):
```
| script | Setup Fixture |
| set maczkopat code | MACZ-001 |
| set maczkopat capacity | 10 |
| $maczkopatId= | create maczkopat |
```

### ParcelFixture
Executes parcel commands and queries:
```
| script | Parcel Fixture |
| check | update parcels to transit | PCK-001, PCK-002 | true |
| check | parcel has status | PCK-001 | InTransit | true |
```

### MaczkopatFixture
Manages maczkopat operations:
```
| script | Maczkopat Fixture |
| select maczkopat | $maczkopatId |
| check | get parcels in maczkopat count | $maczkopatId | 2 |
```

### AuthorizationFixture
Simulates user authentication:
```
| script | Authorization Fixture |
| login with role | Kurier |
| check | is authenticated | true |
| check | can access courier functions | true |
```

### NotificationFixture
Verifies email notifications:
```
| script | Notification Fixture |
| check | in maczkopat notification sent | true |
| check | notification sent to | user@test.com | true |
```

## Test Cases

### PU07 - Odbiór paczek z magazynu
Tests the process of:
1. Courier authorization
2. Loading parcels from warehouse to maczkopat
3. Status updates (InWarehouse → InTransit → InMaczkopat)
4. Customer notifications
5. Blocking unauthorized actions

### PU08 - Zawożenie paczek do magazynu
Tests the process of:
1. Courier authorization
2. Emptying maczkopat lockers
3. Status updates (InMaczkopat → SendToStorage)
4. Customer notifications
5. Blocking unauthorized actions

## Troubleshooting

### "No handler found" error
Make sure the project is built and the assembly path in config is correct.

### Port already in use
Change the port: `java -jar fitnesse-standalone.jar -p 8081`

### Tests not finding fixtures
1. Check namespace in SetUp page matches fixture namespace
2. Verify assembly path in suite.config.xml
3. Ensure project references are correct

## Commands Used in Tests

The tests use the following commands and queries from the Core project:

| Component | Type | Description |
|-----------|------|-------------|
| `BulkUpdateParcelStatusCommand` | Command | Updates parcel status and sends notifications |
| `GetParcelsFromMaczkopatQuery` | Query | Gets parcels assigned to a maczkopat |
| `TrackParcelByFriendlyIdQuery` | Query | Gets parcel event logs |
| `MaczkopatAddLogCommand` | Command | Adds log entry to maczkopat |

## ParcelStatus Values

| Status | Polish Translation | Description |
|--------|-------------------|-------------|
| InWarehouse | W magazynie | Parcel in warehouse |
| InTransit | W tranzycie | Parcel being transported |
| InMaczkopat | W maczkopacie | Parcel in locker |
| SendToStorage | Wysłana do magazynu | Parcel sent to storage |
| Delivered | Dostarczona | Parcel delivered |

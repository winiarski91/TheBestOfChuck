# The Best of Chuck

Azure Function that periodically fetches Chuck Norris jokes from RapidAPI, filters them by length, and stores unique jokes in SQLite database.

## Quick Start

### 1. Configure API and Schedule
Create or update `local.settings.json`:
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "ChuckNorrisApi:Url": "https://matchilling-chuck-norris-jokes-v1.p.rapidapi.com/jokes/random",
    "ChuckNorrisApi:ApiKey": "your-api-key",
    "JokeFetcher:Schedule": "0 */5 * * * *",
    "JokeFetcher:Count": "5"
  },
  "ConnectionStrings": {
    "JokesDatabase": "Data Source=Data/chuckjokes.db"
  }
}
```

### 2. Database Setup
```bash
dotnet tool install --global dotnet-ef
dotnet ef database update
```

## Configuration Options

### Timer Schedule (JokeFetcher:Schedule)
- Every 5 minutes: `0 */5 * * * *`
- Every hour: `0 0 */1 * * *`
- Daily at midnight: `0 0 0 * * *`

### Local Debugging
Add `RunOnStartup = true` to the TimerTrigger attribute in JokeFetcherFunction to run the function immediately when debugging:
```csharp
public async Task Run([TimerTrigger("%JokeFetcher:Schedule%", RunOnStartup = true)] FunctionContext context)
```

### Other Settings
- `JokeFetcher:Count`: Number of jokes to fetch per run
- `ChuckNorrisApi:ApiKey`: Your RapidAPI key
- `JokesDatabase`: SQLite database path

The function will automatically:
- Fetch jokes on the configured schedule
- Filter out jokes longer than 200 characters
- Prevent duplicate jokes using hash values
- Store valid jokes in SQLite database

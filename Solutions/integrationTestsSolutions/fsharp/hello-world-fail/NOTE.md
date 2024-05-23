The exercism/fsharp-runner image only accepts fsprojects, solutions and tests that are named like exercise parameter.

For example,
Exercise = Solution

- Solution.fs
- Tests.fs (won't work, has to be ${Exercise}Tests.fs)

Project name has to be ${Exercise}.

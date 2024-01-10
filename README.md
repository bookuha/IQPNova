## IQP (server)

### Description

IQP (*Interview Questions Portal/Practice*) is a web application that allows you to practice your tech interviewing skills by:
- Answering **theoretical** interview questions (like "What is the difference between `let` and `const`?")
- Solving **coding** problems (LeetCode like) 
- Passing complex **test** tasks (like "Create a simple REST API")

### What is done

- **User.** Registration, authentication and authorization (two layered) (JWT, but without refresh tokens), administrators, etc.
- **Questions.** CRUD, likes, comments, categories, special BL, etc.
- **Coding Problems.** CRUD, submission, test submission, categories, just running arbitrary code (for FE purposes), adding new languages, special BL, etc.

### Installation

There is no docker support yet, so you need to have your own Postgres instance running.
Also, you need to have Docker installed on your machine for running code submissions.

1. Clone the repo
2. Make sure you have Postgres & Docker running
3. Set up appsettings.json (database connection string)
4. Update database (e.g. `dotnet ef database update`)
5. Enjoy!

### Documentation

More documentation is coming soon. For now, you can contact me in Telegram: [@sharpenjoyer](https://t.me/sharpenjoyer) or GitHub issues.

I am planning to add more documentation on Code Running process specifically. Also, you may want to check TODO.md.

### Thanks

Thanks to Exercism for sharing materials on how to run code submissions! At the moment the code running process is based entirely on their Docker code runners.
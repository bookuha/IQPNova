## IQP (server)

### Description

IQP (*Interview Questions Portal/Practice*) is a web application that allows you to practice your tech interviewing skills by:
- Answering **theoretical** interview questions (like "What is the difference between `let` and `const`?")
- Solving **coding** problems (LeetCode like: "Reverse integer", "Validate palindrome")

The content of the platform is populated by users (theory) and special admin users (coding problems).  

### What is done

- **Users.** Registration, authentication and authorization (JWT), administrators, etc.
- **Questions.** CRUD, likes, comments, categories, special BL, etc.
- **Coding Problems.** CRUD, submission, test submission, categories, adding new languages, special BL, etc.

### Installation

There are two ways to setup the project:

Docker: run docker compose up while in the root directory of the project

Terraform with DigitalOcean: 
1) Go to the terraform/digital_ocean directory
2) Obtain your DigitalOcean Private Access Token and SSH private key. Set them up conveniently, such as by creating a terraform.tfvars file and populating it with the necessary values. Refer to the [Terraform documentation](https://registry.terraform.io/providers/terraform-redhat/rhcs/latest/docs/guides/terraform-vars) for guidance.
3) Execute terraform init, terraform plan (optional) and terraform apply to deploy the project


### Documentation

More documentation is coming soon. For now, you can contact me in Telegram: [@sharpenjoyer](https://t.me/sharpenjoyer) or GitHub issues.

I am planning to add more documentation on Code Running process specifically.

### Thanks

Thanks to Exercism for sharing materials on how to run code submissions!

# Todo List Application 

Todo List Application is a productivity tool built with C# that equips users with the capabilities to create, manage, and track tasks or todo items. The application provides both mobile and web-based interfaces, thereby offering users access to their tasks from anywhere. It is deployed to Azure and aligns with the rules of the Well Architected Framework (WAF). 

## Features
1. **Task Management**: Users can add, edit, and delete tasks. 
2. **Due Dates and Reminders**: Users can set due dates for tasks and create reminders.
3. **Task Categorization**: Users can categorize tasks based on projects or priority.
4. **Task Completion**: Users can mark tasks as complete once they're done.
5. **Collaboration**: Users can share tasks with others or assign tasks to team members. 
   
## Architecture

The application's architecture adheres to Microsoft Azure's Well-Architected Framework and follows the provided architectural guidelines. 

- **Scaling**: The application is designed for scaling to meet user demands efficiently. The application and the services it uses are stateless, enabling requests to be routed to any instance.

- **Partitioning**: The workload of the application is partitioned to maximize use of each compute unit. This partitioning also allows the application to scale by adding instances of specific resources.

- **Automated Operational Tasks**: Azure Functions is used to automate operational tasks thereby reducing manual interventions and risks, freeing up human capacity for further innovation.

- **Language & SDK**: The application is developed in C# and primarily utilizes the .NET Azure SDKs for reliability, performance, and frequent feature updates. 

- **Compute Option**: The compute option of the application has been evaluated based on the chosen programming language. 

## Running the Application

To run the application, follow the steps below:

1. Clone the repository to your local machine
2. Navigate to the directory of the cloned repository
3. Run the application:

```bash
dotnet run
```

## Performance Efficiency

This application is designed with performance efficiency in mind, one of the pillars of the Microsoft Azure Well-Architected Framework. The application is designed for scaling and partitioning the workload to provide maximum performance.

## Security

The application follows the security guidelines prescribed by the Microsoft Azure Well-Architected Framework, thereby ensuring that your data is secure and privacy is maintained.

## Contributing

Contributions are welcome. Please read the contribution guidelines to get started.

## License

The Todo List Application is open-source software licensed under the MIT license.
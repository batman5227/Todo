# TODO - 100% couverture lignes (ligne coverage)

- [ ] Identifier les lignes non couvertes (matrice code coverage) pour src (Todo.Api / Application / Domain / Infrastructure / TodoController)
- [ ] Ajouter/ajuster des tests pour couvrir :
  - [ ] `Todo.Domain.Models.MatheService` branches (WorkToday true/false)
  - [ ] `Todo.Domain.Models.Mathe.Factorial` branches (0/1, n>1, exceptions)
  - [ ] `Todo.Domain.Models.TodoItem.Delete()` branche NotImplemented
  - [ ] `Todo.Api.Controllers.TodoController` paths catch & filter switch
  - [ ] `Todo.Application` handlers/validators/mediator/pipeline behaviour
  - [ ] `Todo.Infrastructure` repository branches (isCompleted.HasValue, foreach, empty completed list)
- [ ] Exécuter `dotnet test` avec coverlet pour régénérer le rapport
- [ ] Répéter jusqu’à 100% de coverage ligne


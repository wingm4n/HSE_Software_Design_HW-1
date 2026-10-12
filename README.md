# HSE_Software_Design_HW-1
### Коростелев Александр, студент БПИ-258

## Используемые технологии
**Stack**: C# on .NET 10 (SDK-style project), console application\
**Libraries**: Microsoft.Extensions.DependencyInjection (DI container), xUnit + Moq (unit testing)

**Tests coverage check**: Cobertura framework, XML\
(**Result**: Line coverage **95.09%**, Branch coverage **96.46%**)

## Быстрый старт
Из корневой директории решения


**Запуск  приложения:** 
```
dotnet build
dotnet run --project src/VyshKat
```

**Запуск тестов**: 
```
dotnet test
```

**Проверка покрытия тестами**: 
```
dotnet test --collect:"XPlat Code Coverage"
```

## Решения, принятые в ходе проектирования:  
- разделяю бизнес-логику с абстракциями предметной области (Application + Domain) и консольный интерфейс (Console) - позволяет в будущем изменить UI на любой другой, не меняя бизнес-логику; разделение Application и Domain позволяет добавлять новые типы объектов / новые бизнес-правила, не нарушая OCP. 
- Явный CompositionRoot в Program.cs .
- IServiceCenter чтобы мокать и расширять.
- FleetService реализует основную бизнес-логику парка.

## Соблюдение принципов SOLID + DI, обоснование
SOLID + DI: DI явный, SOLID - L не отслеживается, но поддерживается в Things, может нарушаться в Transport - самый слабый. S + I среди Things и Тransport (и всеми интерфейсами) = O (сильный). D - сквозной, везде используются интерфейсы и абстрактные классы, а не сами предметы.


## Использование ИИ 
### No code: 
| Факт использования | Модель |
| :--- | :--- |
| Обсуждение структуризации решения (разбиение на папки и файлы проекта) | DeepSeek V4 |
| Проверка решения на соответствие установленным в задании требованиям  | Claude Sonnet 5.5 |

### Code:
| Факт использования | Модель |
| :--- | :--- |
| Создание однотипных классов по заданному образцу (Transports, Things) | DeepSeek V4, Claude Sonnet 5.5 |
| Создание расширенных тестовых случаев, увеличение процента покрытия тестами (Tests) | DeepSeek V4, Claude Sonnet 5.5 |
| Разработка консольного интерфейса (оболочка для взаимодействия с бизнес-логикой) (Console) | Claude Sonnet 5.5 |
| Внесение общих незначительных правок в код (naming, codestyle, модификаторы)| Claude Sonnet 5.5 |
| Наполнение комментариями и документирование в коде| Claude Sonnet 5.5 |


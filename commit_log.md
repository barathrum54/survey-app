### Commit: 94c0bca
- Author: barathrum54
- Date: 2025-04-20 14:45:12 +0300

chore: post-flush


---
### Commit: 92b53e9
- Author: barathrum54
- Date: 2025-04-20 15:10:12 +0300

feat: implement JWT token service with config and DI support

- Added JwtSettings config model and wired into appsettings.json
- Created IJwtTokenService interface and JwtTokenService implementation
- Registered JwtTokenService in DI
- Added Microsoft.AspNetCore.Authentication.JwtBearer and related packages
- Initialized JWT options and middleware in Program.cs

---
### Commit: 7ecd3ec
- Author: Taha
- Date: 2025-04-20 15:12:15 +0300

Merge pull request #44 from barathrum54/7-add-jwt-packages

feat: implement JWT token service with config and DI support
---
### Commit: df256f4
- Author: barathrum54
- Date: 2025-04-20 16:25:12 +0300

chore: move .git root and add test project with solution structure


---
### Commit: 0c1c9c9
- Author: barathrum54
- Date: 2025-04-20 16:31:36 +0300

test: ensure valid key length for JWT HMAC signing in JwtTokenServiceTests

Updated test secret key to meet HS256 minimum requirement (256 bits).

---
### Commit: 4e864ee
- Author: Taha
- Date: 2025-04-20 16:31:55 +0300

Merge pull request #45 from barathrum54/8-create-token-generation-logic

8 create token generation logic
---
### Commit: ec0912d
- Author: barathrum54
- Date: 2025-04-20 17:41:25 +0300

feat(jwt): add and validate JWT middleware with protected route and integration tests


---
### Commit: 78850de
- Author: Taha
- Date: 2025-04-20 17:41:41 +0300

Merge pull request #46 from barathrum54/9-add-middleware-for-jwt

feat(jwt): add and validate JWT middleware with protected route and iÔÇĞ
---
### Commit: be2c1dc
- Author: barathrum54
- Date: 2025-04-20 17:46:42 +0300

test: add unit test for UserDao.GetByUsername with existing user


---
### Commit: 0fcedf4
- Author: Taha
- Date: 2025-04-20 17:46:58 +0300

Merge pull request #47 from barathrum54/10-design-user-entity

test: add unit test for UserDao.GetByUsername with existing user
---
### Commit: aba5786
- Author: barathrum54
- Date: 2025-04-20 18:17:02 +0300

feat(auth): implement user registration with validation, hashing, and database integration

- Add RegisterRequest DTO and ApiResponse wrapper
- Implement /auth/register endpoint with validation for existing users
- Store hashed passwords using IPasswordHasher
- Extend User model and SQL mapping with Email field
- Add integration test for successful registration

---
### Commit: c46f616
- Author: Taha
- Date: 2025-04-20 18:17:22 +0300

Merge pull request #48 from barathrum54/11-create-register-endpoint

feat(auth): implement user registration with validation, hashing, andÔÇĞ
---
### Commit: 66705c1
- Author: barathrum54
- Date: 2025-04-20 19:20:06 +0300

feat(auth): implement login endpoint with password hashing and JWT

- Add POST /auth/login logic in AuthController with credential validation
- Integrate SHA256 password verification in PasswordHasher
- Add LoginRequest DTO for login payload
- Add tests: valid + invalid login scenarios using real DB data
- Ensure all AuthController tests pass with detailed output

---
### Commit: 4faee58
- Author: Taha
- Date: 2025-04-20 19:20:19 +0300

Merge pull request #49 from barathrum54/12-create-login-endpoint

feat(auth): implement login endpoint with password hashing and JWT
---
### Commit: 179d4b1
- Author: barathrum54
- Date: 2025-04-20 20:04:40 +0300

feat(survey): add Survey entity with DAO, XML mapping, and tests

- Created `Survey` model with Title, CreatedBy, CreatedAt fields
- Implemented `ISurveyDao` and `SurveyDao` with insert/getAll/getById methods
- Added SQL mapping in `Surveys.xml` with insert and select statements
- Registered `SurveyDao` in DI container
- Included basic DAO unit test `SurveyDaoTests`
- Updated `SqlMap.config` to include Surveys mapping

---
### Commit: df3a673
- Author: Taha
- Date: 2025-04-20 20:05:01 +0300

Merge pull request #50 from barathrum54/13-add-survey-entity

feat(survey): add Survey entity with DAO, XML mapping, and tests
---
### Commit: b264a8d
- Author: barathrum54
- Date: 2025-04-20 21:43:33 +0300

feat(option): add Option entity with DAO, SQL mapping, and tests

- Created Option model with Id, Text, SurveyId, CreatedAt fields
- Implemented IOptionDao and OptionDao with insert/get methods
- Added iBatis XML mapping for Option (insert, getById, getAll)
- Registered OptionDao in Program.cs DI container
- Added unit test for OptionDao insert and retrieval
- Extended TestController with manual Option insert

---
### Commit: 465bb60
- Author: Taha
- Date: 2025-04-20 22:06:07 +0300

Merge pull request #51 from barathrum54/14-add-option-entity

feat(option): add Option entity with DAO, SQL mapping, and tests
---
### Commit: b9f9ac5
- Author: barathrum54
- Date: 2025-04-21 00:20:08 +0300

feat(survey): implement survey creation logic with validation and tests

- Added SurveyController endpoint: POST /survey
- Implemented SurveyService with option count validation
- Integrated OptionDao for option persistence
- Extended JWT claims to include user ID (NameIdentifier)
- Introduced CreateSurveyRequest DTO with validation attributes
- Covered service & controller with unit + integration tests
- Added HttpClient extension for login + token auth in tests

---
### Commit: 747c040
- Author: Taha
- Date: 2025-04-21 00:21:19 +0300

Merge pull request #52 from barathrum54/17-implement-create-survey-logic

feat(survey): implement survey creation logic with validation and tests
---
### Commit: d315256
- Author: barathrum54
- Date: 2025-04-21 00:34:18 +0300

feat(survey): implement GET /surveys/{id} to fetch survey with options

- Added GetSurveyById endpoint in SurveyController
- Introduced SurveyWithOptionsResponse DTO for composite response
- Extended ISurveyService and SurveyService with GetById method
- Added Options.GetBySurveyId iBATIS query mapping
- Updated OptionDao with GetBySurveyId method
- Added unit tests for valid and not found survey cases

---
### Commit: 09b4423
- Author: Taha
- Date: 2025-04-21 00:34:31 +0300

Merge pull request #53 from barathrum54/19-get-survey-by-id

feat(survey): implement GET /surveys/{id} to fetch survey with options
---
### Commit: debedf7
- Author: barathrum54
- Date: 2025-04-21 00:52:30 +0300

feat(surveys): implement GET /surveys/me to list user-owned surveys

- Added GetSurveysByUserId to ISurveyService and SurveyService
- Introduced new iBatis query: Surveys.GetByUserId
- Updated SurveyController with GET /surveys/me endpoint
- Added integration test: GetMySurveys_ShouldReturnOnlyOwnedSurveys
- Fixed test to assert by user ID (not username)

---
### Commit: 9b4fe69
- Author: Taha
- Date: 2025-04-21 00:52:52 +0300

Merge pull request #54 from barathrum54/20-list-surveys-by-user

feat(surveys): implement GET /surveys/me to list user-owned surveys
---
### Commit: e1aa38b
- Author: barathrum54
- Date: 2025-04-21 01:38:57 +0300

Fix: Corrected token issue for attacker user in delete survey test - Updated the login credentials for the attacker user from 'admin2' to 'admin' - Added token logging in HttpClientExtensions for better debugging - Ensured correct token handling in the DeleteSurvey test case


---
### Commit: da47841
- Author: Taha
- Date: 2025-04-21 01:39:26 +0300

Merge pull request #55 from barathrum54/21-delete-survey-if-owner

Fix: Corrected token issue for attacker user in delete survey test
---
### Commit: e101a64
- Author: barathrum54
- Date: 2025-04-21 02:18:50 +0300

feat(vote): implement vote entity, service, and controller

- Added Vote entity with relevant properties (UserId, SurveyId, OptionId).
- Implemented VoteService for handling vote logic.
- Created VoteController for casting votes and retrieving survey vote results.
- Updated DAOs to support vote operations.
- Added integration tests for vote creation and result retrieval.

---
### Commit: 4b45140
- Author: Taha
- Date: 2025-04-21 02:19:08 +0300

Merge pull request #56 from barathrum54/22-design-vote-entity

feat(vote): implement vote entity, service, and controller
---
### Commit: 5b6745c
- Author: barathrum54
- Date: 2025-04-21 02:33:34 +0300

feat: implement result aggregation for /survey/{id}/results endpoint with vote counts


---
### Commit: 4a57497
- Author: Taha
- Date: 2025-04-21 02:33:53 +0300

Merge pull request #57 from barathrum54/26-implement-result-aggregation

feat: implement result aggregation for /survey/{id}/results endpoint ÔÇĞ
---
### Commit: 0323e3c
- Author: barathrum54
- Date: 2025-04-21 02:42:21 +0300

feat: add percentage to survey result aggregation response

- Updated SurveyResult DTO to include a Percentage field.
- Modified SurveyController to return percentage in the survey results.
- Enhanced test for survey results to check the correct vote percentage calculation.

---
### Commit: 5a1c90a
- Author: Taha
- Date: 2025-04-21 02:42:35 +0300

Merge pull request #58 from barathrum54/27-create-result-dto

feat: add percentage to survey result aggregation response
---
### Commit: 1fe6cc5
- Author: barathrum54
- Date: 2025-04-21 02:58:18 +0300

feat: add global exception handler to catch unhandled errors

- Implemented a global exception handler using middleware.
- Ensured that all uncaught exceptions are handled gracefully.
- Returns proper error messages and status codes for different types of errors.

---
### Commit: 9eeab3a
- Author: Taha
- Date: 2025-04-21 02:58:35 +0300

Merge pull request #59 from barathrum54/28-add-global-exception-handler

feat: add global exception handler to catch unhandled errors
---
### Commit: 6465efb
- Author: barathrum54
- Date: 2025-04-21 09:15:30 +0300

feat: Configure FluentValidation and add validation to DTOs

- Added FluentValidation integration for request validation in controllers.
- Created custom validators for RegisterRequest, LoginRequest, VoteRequest, and CreateSurveyRequest.
- Updated controllers to use the validators for proper input validation.
- Improved error handling in VoteController for validation failures.

---
### Commit: 34ddb09
- Author: Taha
- Date: 2025-04-21 09:16:00 +0300

Merge pull request #60 from barathrum54/29-configure-fluentvalidation

feat: Configure FluentValidation and add validation to DTOs
---
### Commit: 41637bd
- Author: barathrum54
- Date: 2025-04-21 09:58:20 +0300

feat(swagger): add JWT bearer authentication support

- Configured Swagger to support JWT tokens via 'Authorize' button
- Added global security definitions and requirements for protected endpoints
- Allows testing authenticated routes directly from Swagger UI

---
### Commit: e18a9f5
- Author: Taha
- Date: 2025-04-21 09:58:34 +0300

Merge pull request #65 from barathrum54/32-add-jwt-input-to-swagger

feat(swagger): add JWT bearer authentication support
---
### Commit: f01e865
- Author: barathrum54
- Date: 2025-04-21 10:02:18 +0300

feat: add /healthz endpoint for readiness probe


---
### Commit: b62749b
- Author: Taha
- Date: 2025-04-21 10:02:33 +0300

Merge pull request #66 from barathrum54/33-create-healthcheck-endpoint

feat: add /healthz endpoint for readiness probe
---
### Commit: 53b8f31
- Author: barathrum54
- Date: 2025-04-21 10:14:09 +0300

test: add integration test for register-then-login success flow as well as full coverage of auth suite


---
### Commit: 767095d
- Author: Taha
- Date: 2025-04-21 10:14:25 +0300

Merge pull request #67 from barathrum54/34-write-unit-tests-for-auth

test: add integration test for register-then-login success flow as weÔÇĞ
---
### Commit: b52c9cd
- Author: barathrum54
- Date: 2025-04-21 10:43:49 +0300

refactor(survey): return SurveyWithOptionsResponse from CreateSurvey

- Updated SurveyService and interface to return full SurveyWithOptionsResponse after creation
- Modified controller endpoint to reflect new return model
- Adjusted test assertions to validate options on creation

---
### Commit: 049ffac
- Author: Taha
- Date: 2025-04-21 10:44:12 +0300

Merge pull request #69 from barathrum54/35-write-unit-tests-for-survey

refactor(survey): return SurveyWithOptionsResponse from CreateSurvey
---
### Commit: 4d51223
- Author: barathrum54
- Date: 2025-04-21 10:57:43 +0300

test: add integration tests for vote endpoint + support autoRegister in LoginAndGetTokenAsync


---
### Commit: 4b16ed7
- Author: Taha
- Date: 2025-04-21 10:58:00 +0300

Merge pull request #73 from barathrum54/36-write-integration-test-for-vote

test: add integration tests for vote endpoint + support autoRegister ÔÇĞ
---
### Commit: f2de436
- Author: barathrum54
- Date: 2025-04-21 11:23:27 +0300

docs(swagger): Add comprehensive Swagger annotations to controllers

- Documented all Auth, Survey, and Vote controller endpoints with summary, remarks, response types, and route metadata.
- Improved Swagger UI clarity and endpoint discoverability.
- Marked internal TestController as hidden from Swagger docs.

---
### Commit: 4b52989
- Author: Taha
- Date: 2025-04-21 11:23:53 +0300

Merge pull request #74 from barathrum54/61-create-swagger-annotations-with-full-coverage

docs(swagger): Add comprehensive Swagger annotations to controllers
---
### Commit: 6f22aee
- Author: barathrum54
- Date: 2025-04-21 11:26:07 +0300

fix: remove unnecessary usage of results in vote domain - moved to survey domain


---
### Commit: 200027a
- Author: barathrum54
- Date: 2025-04-21 11:39:37 +0300

fix: make routes lowercase fix: adjust test suites for survey & vote


---
### Commit: efb8f96
- Author: Taha
- Date: 2025-04-21 11:40:28 +0300

Merge pull request #75 from barathrum54/62-survey-results-endpoint-should-return-info-message-if-there-is-no-vote

62 survey results endpoint should return info message if there is no vote
---
### Commit: b880c3e
- Author: barathrum54
- Date: 2025-04-21 11:48:19 +0300

feat(auth): return JWT token on successful registration and verify token validity in integration tests

- Updated register endpoint to return JWT token upon success
- Added integration test to validate returned token grants access to protected endpoints

---
### Commit: a249e6e
- Author: Taha
- Date: 2025-04-21 11:48:57 +0300

Merge pull request #76 from barathrum54/63-register-endpoint-should-return-token-on-success

feat(auth): return JWT token on successful registration and verify toÔÇĞ
---
### Commit: 8faa9d5
- Author: barathrum54
- Date: 2025-04-21 12:23:42 +0300

feat: createa db backup, .sql file


---
### Commit: 952952d
- Author: Taha
- Date: 2025-04-21 12:24:05 +0300

Merge pull request #77 from barathrum54/64-audit-sql-schemas-for-best-practices-improvements

feat: createa db backup, .sql file
---

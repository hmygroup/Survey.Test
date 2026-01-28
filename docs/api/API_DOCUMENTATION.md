# Survey Application - Complete API Documentation

**Version:** 1.0  
**Date:** January 2026  
**API Base URL:** `http://localhost:5049/api/`  
**Connection ID:** `10001`

---

## Table of Contents

1. [Overview](#overview)
2. [Connection Configuration](#connection-configuration)
3. [Questionary Endpoints](#questionary-endpoints)
4. [Question Endpoints](#question-endpoints)
5. [Answer Endpoints](#answer-endpoints)
6. [Question Response Endpoints](#question-response-endpoints)
7. [Question Type Endpoints](#question-type-endpoints)
8. [Policy Endpoints](#policy-endpoints)
9. [Attachment Endpoints](#attachment-endpoints)
10. [Complete Workflow Examples](#complete-workflow-examples)
11. [Error Handling](#error-handling)

---

## Overview

This document provides comprehensive documentation for all API endpoints in the Survey Management System. All endpoints use the same **Connection ID: 10001** for consistency across the application.

### Key Concepts

- **Questionary**: A survey/questionnaire definition
- **Question**: Individual questions within a questionary
- **Answer**: A user's session when filling out a survey (contains metadata)
- **QuestionResponse**: Individual answers to specific questions within an Answer session
- **Constraint**: Validation rules applied to questions via Policies
- **Policy**: Reusable validation rule templates

---

## Connection Configuration

**Base URL**: `http://localhost:5049/api/`  
**Connection ID**: `10001`  
**Content-Type**: `application/json`  
**Authentication**: Not required in development environment

All API endpoints require the `connection` parameter set to `10001` to identify the multi-tenant database.

---

## Questionary Endpoints

### 1. Get All Questionnaires

Retrieves all questionnaires from the system.

**Endpoint**: `GET /api/questionary/10001/all`

**Parameters**:
- `connection`: `10001` (path parameter)

**Response**: `List<QuestionaryDto>`

```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Customer Satisfaction Survey",
    "description": "Annual customer feedback",
    "createdBy": "admin@company.com",
    "creationDate": "2026-01-15T10:00:00Z"
  },
  {
    "id": "650e8400-e29b-41d4-a716-446655440001",
    "name": "Employee Engagement Survey",
    "description": "Quarterly employee feedback",
    "createdBy": "hr@company.com",
    "creationDate": "2026-01-20T14:30:00Z"
  }
]
```

**Example Usage**:
```bash
curl -X GET "http://localhost:5049/api/questionary/10001/all" \
  -H "Content-Type: application/json"
```

---

### 2. Get Questionary by ID

Retrieves a specific questionary by its ID.

**Endpoint**: `GET /api/Questionary/10001/{id}`

**Parameters**:
- `connection`: `10001` (path parameter)
- `id`: Guid (path parameter)

**Response**: `QuestionaryDto`

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Customer Satisfaction Survey",
  "description": "Annual customer feedback",
  "createdBy": "admin@company.com",
  "creationDate": "2026-01-15T10:00:00Z"
}
```

**Example Usage**:
```bash
curl -X GET "http://localhost:5049/api/Questionary/10001/550e8400-e29b-41d4-a716-446655440000" \
  -H "Content-Type: application/json"
```

---

### 3. Get Questionary by Name

Retrieves a questionary by its name.

**Endpoint**: `GET /api/Questionary/10001/name/{name}`

**Parameters**:
- `connection`: `10001` (path parameter)
- `name`: string (path parameter, URL-encoded)

**Response**: `QuestionaryDto`

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Customer Satisfaction Survey",
  "description": "Annual customer feedback",
  "createdBy": "admin@company.com",
  "creationDate": "2026-01-15T10:00:00Z"
}
```

**Example Usage**:
```bash
curl -X GET "http://localhost:5049/api/Questionary/10001/name/Customer%20Satisfaction%20Survey" \
  -H "Content-Type: application/json"
```

---

### 4. Get Full Questionary (with Questions)

Retrieves a complete questionary including all questions, constraints, and related data (but without response data).

**Endpoint**: `GET /api/questionary/10001/{id}/full`

**Parameters**:
- `connection`: `10001` (path parameter)
- `id`: Guid (path parameter)

**Response**: `FullQuestionaryDto`

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Customer Satisfaction Survey",
  "description": "Annual customer feedback",
  "questions": [
    {
      "id": "750e8400-e29b-41d4-a716-446655440002",
      "questionText": "How satisfied are you with our service?",
      "questionType": {
        "id": "850e8400-e29b-41d4-a716-446655440003",
        "dotNetType": "System.String"
      },
      "constraints": [
        {
          "id": "950e8400-e29b-41d4-a716-446655440004",
          "questionId": "750e8400-e29b-41d4-a716-446655440002",
          "policy": {
            "id": "a50e8400-e29b-41d4-a716-446655440005",
            "name": "Required Field"
          },
          "policyRecords": [
            {
              "id": "b50e8400-e29b-41d4-a716-446655440006",
              "constraintId": "950e8400-e29b-41d4-a716-446655440004",
              "value": "required:true"
            }
          ]
        }
      ],
      "questionResponses": [],
      "distinctAnswers": []
    }
  ]
}
```

**Example Usage**:
```bash
curl -X GET "http://localhost:5049/api/questionary/10001/550e8400-e29b-41d4-a716-446655440000/full" \
  -H "Content-Type: application/json"
```

---

### 5. Create New Questionary

Creates a new questionary with the specified name.

**Endpoint**: `POST /api/Questionary/10001/New/{name}`

**Parameters**:
- `connection`: `10001` (path parameter)
- `name`: string (path parameter, URL-encoded, max 255 characters)

**Response**: `QuestionaryDto`

```json
{
  "id": "c50e8400-e29b-41d4-a716-446655440007",
  "name": "New Employee Survey",
  "description": null,
  "createdBy": "system",
  "creationDate": "2026-01-28T10:00:00Z"
}
```

**Example Usage**:
```bash
curl -X POST "http://localhost:5049/api/Questionary/10001/New/New%20Employee%20Survey" \
  -H "Content-Type: application/json"
```

---

## Question Endpoints

### 1. Get Questions by Questionary ID

Retrieves all questions for a specific questionary.

**Endpoint**: `GET /api/question/10001/get?questionaryId={questionaryId}`

**Parameters**:
- `connection`: `10001` (path parameter)
- `questionaryId`: Guid (query parameter)

**Response**: `List<QuestionDto>`

```json
[
  {
    "id": "750e8400-e29b-41d4-a716-446655440002",
    "questionText": "How satisfied are you with our service?",
    "questionType": {
      "id": "850e8400-e29b-41d4-a716-446655440003",
      "dotNetType": "System.String"
    },
    "constraints": [],
    "questionResponses": [],
    "distinctAnswers": []
  }
]
```

**Example Usage**:
```bash
curl -X GET "http://localhost:5049/api/question/10001/get?questionaryId=550e8400-e29b-41d4-a716-446655440000" \
  -H "Content-Type: application/json"
```

---

### 2. Create Questions

Creates one or more questions for a questionary.

**Endpoint**: `POST /api/question/new/10001?questionaryId={questionaryId}`

**Parameters**:
- `connection`: `10001` (path parameter)
- `questionaryId`: Guid (query parameter)

**Request Body**: `List<QuestionCreationDto>`

```json
[
  {
    "id": "d50e8400-e29b-41d4-a716-446655440008",
    "questionText": "What is your name?",
    "questionType": {
      "id": "850e8400-e29b-41d4-a716-446655440003",
      "dotNetType": "System.String"
    },
    "constraints": [
      {
        "id": "e50e8400-e29b-41d4-a716-446655440009",
        "questionId": "d50e8400-e29b-41d4-a716-446655440008",
        "policy": {
          "id": "a50e8400-e29b-41d4-a716-446655440005",
          "name": "Required Field"
        },
        "policyRecords": [
          {
            "id": "f50e8400-e29b-41d4-a716-44665544000a",
            "constraintId": "e50e8400-e29b-41d4-a716-446655440009",
            "value": "required:true"
          }
        ]
      }
    ]
  }
]
```

**Response**: `List<QuestionDto>`

```json
[
  {
    "id": "d50e8400-e29b-41d4-a716-446655440008",
    "questionText": "What is your name?",
    "questionType": {
      "id": "850e8400-e29b-41d4-a716-446655440003",
      "dotNetType": "System.String"
    },
    "constraints": [
      {
        "id": "e50e8400-e29b-41d4-a716-446655440009",
        "questionId": "d50e8400-e29b-41d4-a716-446655440008",
        "policy": {
          "id": "a50e8400-e29b-41d4-a716-446655440005",
          "name": "Required Field"
        },
        "policyRecords": [
          {
            "id": "f50e8400-e29b-41d4-a716-44665544000a",
            "constraintId": "e50e8400-e29b-41d4-a716-446655440009",
            "value": "required:true"
          }
        ]
      }
    ],
    "questionResponses": [],
    "distinctAnswers": []
  }
]
```

**Example Usage**:
```bash
curl -X POST "http://localhost:5049/api/question/new/10001?questionaryId=550e8400-e29b-41d4-a716-446655440000" \
  -H "Content-Type: application/json" \
  -d '[{"id":"d50e8400-e29b-41d4-a716-446655440008","questionText":"What is your name?","questionType":{"id":"850e8400-e29b-41d4-a716-446655440003","dotNetType":"System.String"},"constraints":[]}]'
```

---

### 3. Get Questions with Specific Responses

Retrieves questions along with specific answer responses.

**Endpoint**: `POST /api/question/10001?questionaryId={questionaryId}`

**Parameters**:
- `connection`: `10001` (path parameter)
- `questionaryId`: Guid (query parameter)

**Request Body**: `List<Guid>` (List of Answer IDs)

```json
[
  "123e4567-e89b-12d3-a456-426614174000",
  "223e4567-e89b-12d3-a456-426614174001"
]
```

**Response**: `List<QuestionDto>` with populated questionResponses

**Example Usage**:
```bash
curl -X POST "http://localhost:5049/api/question/10001?questionaryId=550e8400-e29b-41d4-a716-446655440000" \
  -H "Content-Type: application/json" \
  -d '["123e4567-e89b-12d3-a456-426614174000"]'
```

---

## Answer Endpoints

### 1. Create Answer Session

Creates a new answer session when a user starts filling out a questionary.

**Endpoint**: `POST /api/answer/10001?questionaryId={questionaryId}&user={user}&cardId={cardId}`

**Parameters**:
- `connection`: `10001` (path parameter)
- `questionaryId`: Guid (query parameter)
- `user`: string (query parameter) - email or username
- `cardId`: int (query parameter, optional)

**Response**: `AnswerDto`

```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "questionaryId": "550e8400-e29b-41d4-a716-446655440000",
  "user": "john.doe@company.com",
  "cardId": 12345,
  "answerStatus": {
    "id": "323e4567-e89b-12d3-a456-426614174002",
    "name": "UNFINISHED",
    "answer_status": "UNFINISHED"
  }
}
```

**Example Usage**:
```bash
curl -X POST "http://localhost:5049/api/answer/10001?questionaryId=550e8400-e29b-41d4-a716-446655440000&user=john.doe@company.com&cardId=12345" \
  -H "Content-Type: application/json"
```

---

### 2. Get Answer by ID

Retrieves a specific answer session by its ID.

**Endpoint**: `GET /api/answer/10001/{id}`

**Parameters**:
- `connection`: `10001` (path parameter)
- `id`: Guid (path parameter)

**Response**: `AnswerDto`

```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "questionaryId": "550e8400-e29b-41d4-a716-446655440000",
  "user": "john.doe@company.com",
  "cardId": 12345,
  "answerStatus": {
    "id": "323e4567-e89b-12d3-a456-426614174002",
    "name": "COMPLETED",
    "answer_status": "COMPLETED"
  }
}
```

**Example Usage**:
```bash
curl -X GET "http://localhost:5049/api/answer/10001/123e4567-e89b-12d3-a456-426614174000" \
  -H "Content-Type: application/json"
```

---

### 3. Set Answer Status

Updates the status of one or more answer sessions.

**Endpoint**: `PUT /api/answer/setStatus`

**Request Body**: `SetAnswerStatusCommand`

```json
{
  "connectionId": 10001,
  "answersId": [
    "123e4567-e89b-12d3-a456-426614174000",
    "223e4567-e89b-12d3-a456-426614174001"
  ],
  "ANSWER_STATUS": "COMPLETED"
}
```

**Valid Status Values**:
- `UNFINISHED` - Survey session started but not completed
- `PENDING` - Survey completed, awaiting review
- `COMPLETED` - Survey completed and approved
- `CANCELLED` - Survey session cancelled

**Response**: `Unit` (void/empty)

**Example Usage**:
```bash
curl -X PUT "http://localhost:5049/api/answer/setStatus" \
  -H "Content-Type: application/json" \
  -d '{"connectionId":10001,"answersId":["123e4567-e89b-12d3-a456-426614174000"],"ANSWER_STATUS":"COMPLETED"}'
```

---

## Question Response Endpoints

### 1. Save Question Responses

Saves responses to questions within an answer session.

**Endpoint**: `POST /api/questionresponse/10001/response?questionaryId={questionaryId}&currentAnswerId={currentAnswerId}&newCurrentAnswerStatus={status}&answersId={id1}&answersId={id2}`

**Parameters**:
- `connection`: `10001` (path parameter)
- `questionaryId`: Guid (query parameter)
- `currentAnswerId`: Guid (query parameter)
- `newCurrentAnswerStatus`: string (query parameter) - new status for the answer
- `answersId`: Guid[] (query parameter, can be multiple)

**Request Body**: `List<CreateResponseBody>`

```json
[
  {
    "questionId": "750e8400-e29b-41d4-a716-446655440002",
    "response": "Very satisfied",
    "metadata": "{\"timeSpent\":5000,\"device\":\"desktop\"}"
  },
  {
    "questionId": "760e8400-e29b-41d4-a716-446655440003",
    "response": "john.doe@company.com",
    "metadata": "{\"timeSpent\":3000,\"device\":\"desktop\"}"
  }
]
```

**Response**: `List<QuestionResponseDto>`

```json
[
  {
    "id": "423e4567-e89b-12d3-a456-426614174003",
    "questionId": "750e8400-e29b-41d4-a716-446655440002",
    "answerId": "123e4567-e89b-12d3-a456-426614174000",
    "response": "Very satisfied",
    "metadata": "{\"timeSpent\":5000,\"device\":\"desktop\"}",
    "answers": []
  }
]
```

**Example Usage**:
```bash
curl -X POST "http://localhost:5049/api/questionresponse/10001/response?questionaryId=550e8400-e29b-41d4-a716-446655440000&currentAnswerId=123e4567-e89b-12d3-a456-426614174000&newCurrentAnswerStatus=PENDING&answersId=123e4567-e89b-12d3-a456-426614174000" \
  -H "Content-Type: application/json" \
  -d '[{"questionId":"750e8400-e29b-41d4-a716-446655440002","response":"Very satisfied","metadata":"{}"}]'
```

---

### 2. Update Question Response

Updates a single question response.

**Endpoint**: `PATCH /api/questionresponse/10001/response?questionResponseId={id}&response={value}&metadata={json}`

**Parameters**:
- `connection`: `10001` (path parameter)
- `questionResponseId`: Guid (query parameter)
- `response`: string (query parameter) - new response value
- `metadata`: string (query parameter, optional) - JSON metadata

**Response**: `QuestionResponseDto`

```json
{
  "id": "423e4567-e89b-12d3-a456-426614174003",
  "questionId": "750e8400-e29b-41d4-a716-446655440002",
  "answerId": "123e4567-e89b-12d3-a456-426614174000",
  "response": "Extremely satisfied",
  "metadata": "{\"timeSpent\":5000,\"device\":\"desktop\",\"edited\":true}",
  "answers": []
}
```

**Example Usage**:
```bash
curl -X PATCH "http://localhost:5049/api/questionresponse/10001/response?questionResponseId=423e4567-e89b-12d3-a456-426614174003&response=Extremely%20satisfied&metadata=%7B%22edited%22%3Atrue%7D" \
  -H "Content-Type: application/json"
```

---

### 3. Delete Question Response

Deletes a specific question response.

**Endpoint**: `DELETE /api/questionresponse/10001/{id}`

**Parameters**:
- `connection`: `10001` (path parameter)
- `id`: Guid (path parameter)

**Response**: `bool` (true if deleted successfully)

**Example Usage**:
```bash
curl -X DELETE "http://localhost:5049/api/questionresponse/10001/423e4567-e89b-12d3-a456-426614174003" \
  -H "Content-Type: application/json"
```

---

## Question Type Endpoints

### 1. Get All Question Types

Retrieves all available question types.

**Endpoint**: `GET /api/questiontype/10001/all`

**Parameters**:
- `connection`: `10001` (path parameter)

**Response**: `List<QuestionTypeDto>`

```json
[
  {
    "id": "850e8400-e29b-41d4-a716-446655440003",
    "dotNetType": "System.String"
  },
  {
    "id": "860e8400-e29b-41d4-a716-446655440004",
    "dotNetType": "System.Int32"
  },
  {
    "id": "870e8400-e29b-41d4-a716-446655440005",
    "dotNetType": "System.Boolean"
  }
]
```

**Example Usage**:
```bash
curl -X GET "http://localhost:5049/api/questiontype/10001/all" \
  -H "Content-Type: application/json"
```

---

### 2. Get Question Type by ID

Retrieves a specific question type by its ID.

**Endpoint**: `GET /api/questiontype/10001/{id}`

**Parameters**:
- `connection`: `10001` (path parameter)
- `id`: Guid (path parameter)

**Response**: `QuestionTypeDto`

```json
{
  "id": "850e8400-e29b-41d4-a716-446655440003",
  "dotNetType": "System.String"
}
```

**Example Usage**:
```bash
curl -X GET "http://localhost:5049/api/questiontype/10001/850e8400-e29b-41d4-a716-446655440003" \
  -H "Content-Type: application/json"
```

---

### 3. Add Question Type

Creates a new question type.

**Endpoint**: `POST /api/questiontype/10001/Add?typeClass={class}&typeName={name}`

**Parameters**:
- `connection`: `10001` (path parameter)
- `typeClass`: string (query parameter) - .NET type (e.g., "System.String")
- `typeName`: string (query parameter) - display name (e.g., "Short Text")

**Response**: `QuestionTypeDto`

```json
{
  "id": "880e8400-e29b-41d4-a716-446655440006",
  "dotNetType": "System.Decimal"
}
```

**Example Usage**:
```bash
curl -X POST "http://localhost:5049/api/questiontype/10001/Add?typeClass=System.Decimal&typeName=Decimal%20Number" \
  -H "Content-Type: application/json"
```

---

## Policy Endpoints

### 1. Get All Policies

Retrieves all validation policies.

**Endpoint**: `GET /api/policy/10001/all`

**Parameters**:
- `connection`: `10001` (path parameter)

**Response**: `List<PolicyDto>`

```json
[
  {
    "id": "a50e8400-e29b-41d4-a716-446655440005",
    "name": "Required Field",
    "description": "Ensures the field is not empty"
  },
  {
    "id": "a60e8400-e29b-41d4-a716-446655440006",
    "name": "Email Validation",
    "description": "Validates email format"
  }
]
```

**Example Usage**:
```bash
curl -X GET "http://localhost:5049/api/policy/10001/all" \
  -H "Content-Type: application/json"
```

---

### 2. Get Policy by ID

Retrieves a specific policy by its ID.

**Endpoint**: `GET /api/policy/10001/{id}`

**Parameters**:
- `connection`: `10001` (path parameter)
- `id`: Guid (path parameter)

**Response**: `PolicyDto`

```json
{
  "id": "a50e8400-e29b-41d4-a716-446655440005",
  "name": "Required Field",
  "description": "Ensures the field is not empty"
}
```

**Example Usage**:
```bash
curl -X GET "http://localhost:5049/api/policy/10001/a50e8400-e29b-41d4-a716-446655440005" \
  -H "Content-Type: application/json"
```

---

### 3. Create Policy

Creates a new validation policy.

**Endpoint**: `POST /api/policy/10001/new`

**Parameters**:
- `connection`: `10001` (path parameter)

**Request Body**: `PolicyDto`

```json
{
  "name": "Phone Number Validation",
  "description": "Validates phone number format"
}
```

**Response**: `PolicyDto`

```json
{
  "id": "a70e8400-e29b-41d4-a716-446655440007",
  "name": "Phone Number Validation",
  "description": "Validates phone number format"
}
```

**Example Usage**:
```bash
curl -X POST "http://localhost:5049/api/policy/10001/new" \
  -H "Content-Type: application/json" \
  -d '{"name":"Phone Number Validation","description":"Validates phone number format"}'
```

---

## Attachment Endpoints

### 1. Create Attachment

Creates a new file attachment.

**Endpoint**: `POST /api/attachment/10001/new`

**Parameters**:
- `connection`: `10001` (path parameter)

**Request Body**: `AttachmentDto`

```json
{
  "storage": "azure",
  "blb_Attachment": "base64_encoded_content_here",
  "blb_container": "questionary-files",
  "filename": "survey_results.pdf",
  "contentType": "application/pdf",
  "confirmationFlag": true
}
```

**Response**: `AttachmentDto`

```json
{
  "id": "890e8400-e29b-41d4-a716-446655440008",
  "storage": "azure",
  "blb_Attachment": "base64_encoded_content_here",
  "blb_container": "questionary-files",
  "filename": "survey_results.pdf",
  "contentType": "application/pdf",
  "confirmationFlag": true
}
```

**Example Usage**:
```bash
curl -X POST "http://localhost:5049/api/attachment/10001/new" \
  -H "Content-Type: application/json" \
  -d '{"storage":"azure","blb_container":"questionary-files","filename":"test.pdf","contentType":"application/pdf","confirmationFlag":true}'
```

---

### 2. Get Attachment by ID

Retrieves a specific attachment by its ID.

**Endpoint**: `GET /api/attachment/10001/{id}`

**Parameters**:
- `connection`: `10001` (path parameter)
- `id`: Guid (path parameter)

**Response**: `AttachmentDto`

**Example Usage**:
```bash
curl -X GET "http://localhost:5049/api/attachment/10001/890e8400-e29b-41d4-a716-446655440008" \
  -H "Content-Type: application/json"
```

---

## Complete Workflow Examples

### Workflow 1: Creating a New Survey

```bash
# Step 1: Create a new questionary
curl -X POST "http://localhost:5049/api/Questionary/10001/New/Employee%20Feedback%20Survey" \
  -H "Content-Type: application/json"

# Response: {"id":"550e8400-e29b-41d4-a716-446655440000",...}

# Step 2: Get available question types
curl -X GET "http://localhost:5049/api/questiontype/10001/all" \
  -H "Content-Type: application/json"

# Response: [{"id":"850e8400-e29b-41d4-a716-446655440003","dotNetType":"System.String"},...]

# Step 3: Create questions for the questionary
curl -X POST "http://localhost:5049/api/question/new/10001?questionaryId=550e8400-e29b-41d4-a716-446655440000" \
  -H "Content-Type: application/json" \
  -d '[
    {
      "id":"750e8400-e29b-41d4-a716-446655440002",
      "questionText":"How satisfied are you with your work environment?",
      "questionType":{"id":"850e8400-e29b-41d4-a716-446655440003","dotNetType":"System.String"},
      "constraints":[]
    },
    {
      "id":"760e8400-e29b-41d4-a716-446655440003",
      "questionText":"What is your employee ID?",
      "questionType":{"id":"860e8400-e29b-41d4-a716-446655440004","dotNetType":"System.Int32"},
      "constraints":[]
    }
  ]'

# Response: List of created questions
```

---

### Workflow 2: User Filling Out a Survey

```bash
# Step 1: Get the full questionary with all questions
curl -X GET "http://localhost:5049/api/questionary/10001/550e8400-e29b-41d4-a716-446655440000/full" \
  -H "Content-Type: application/json"

# Response: Full questionary with questions

# Step 2: Create an answer session (user starts survey)
curl -X POST "http://localhost:5049/api/answer/10001?questionaryId=550e8400-e29b-41d4-a716-446655440000&user=john.doe@company.com&cardId=12345" \
  -H "Content-Type: application/json"

# Response: {"id":"123e4567-e89b-12d3-a456-426614174000","answerStatus":{"name":"UNFINISHED"},...}

# Step 3: Save user's responses to questions
curl -X POST "http://localhost:5049/api/questionresponse/10001/response?questionaryId=550e8400-e29b-41d4-a716-446655440000&currentAnswerId=123e4567-e89b-12d3-a456-426614174000&newCurrentAnswerStatus=PENDING&answersId=123e4567-e89b-12d3-a456-426614174000" \
  -H "Content-Type: application/json" \
  -d '[
    {
      "questionId":"750e8400-e29b-41d4-a716-446655440002",
      "response":"Very satisfied",
      "metadata":"{\"timeSpent\":5000}"
    },
    {
      "questionId":"760e8400-e29b-41d4-a716-446655440003",
      "response":"54321",
      "metadata":"{\"timeSpent\":2000}"
    }
  ]'

# Response: List of created question responses

# Step 4: Mark the survey as completed
curl -X PUT "http://localhost:5049/api/answer/setStatus" \
  -H "Content-Type: application/json" \
  -d '{
    "connectionId":10001,
    "answersId":["123e4567-e89b-12d3-a456-426614174000"],
    "ANSWER_STATUS":"COMPLETED"
  }'

# Response: Empty (void)
```

---

### Workflow 3: Reviewing Survey Responses

```bash
# Step 1: Get all questions with specific answer responses
curl -X POST "http://localhost:5049/api/question/10001?questionaryId=550e8400-e29b-41d4-a716-446655440000" \
  -H "Content-Type: application/json" \
  -d '["123e4567-e89b-12d3-a456-426614174000"]'

# Response: Questions with populated questionResponses for the specified answer

# Step 2: Get a specific answer session
curl -X GET "http://localhost:5049/api/answer/10001/123e4567-e89b-12d3-a456-426614174000" \
  -H "Content-Type: application/json"

# Response: Answer details with status
```

---

## Error Handling

### Common HTTP Status Codes

- **200 OK**: Request successful
- **201 Created**: Resource created successfully
- **400 Bad Request**: Invalid input or validation error
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Server error

### Error Response Format

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Name": ["The Name field is required."],
    "QuestionaryId": ["The QuestionaryId field must be a valid GUID."]
  }
}
```

### Best Practices for Error Handling

1. **Always check HTTP status codes** before processing response
2. **Log all API errors** for debugging
3. **Provide user-friendly error messages** in the UI
4. **Implement retry logic** for transient errors
5. **Validate input** before sending to API
6. **Handle network timeouts** gracefully

---

## Summary

This document provides complete API documentation for the Survey Management System. All endpoints use **Connection ID: 10001** for consistency. 

**Key Points**:
- Always include the connection parameter (10001) in all API calls
- Answer represents a user session, QuestionResponse represents individual answers
- Answer status flows: UNFINISHED → PENDING → COMPLETED (or CANCELLED)
- All endpoints return JSON formatted data
- Error responses follow standard HTTP status codes

For implementation details and C# code examples, see `API_INTEGRATION.md` in the SurveyApp folder.

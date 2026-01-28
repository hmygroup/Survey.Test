# Backend API Implementation Requirements

**Project:** Survey Management System  
**Frontend Version:** Phase 4 (40% Complete)  
**Date:** January 28, 2026  
**Priority:** HIGH - Blocking frontend development  
**Connection ID:** 10001  
**Base URL:** `http://localhost:5049/api/`

---

## 🎯 Executive Summary

The WPF frontend application has successfully implemented Phases 1-3 (100%) and Phase 4 (40%). However, further progress is blocked by missing backend API endpoints. This document outlines the **critical and high-priority APIs** needed to complete Phase 4 (Response Collection), Phase 5 (Response Analysis), and Phase 6 (Polish & Optimization).

### Current Frontend Status
- ✅ **Phase 1**: Foundation (COMPLETE)
- ✅ **Phase 2**: Questionary Management (COMPLETE)
- ✅ **Phase 3**: Question Editor (COMPLETE)
  - Drag-drop reordering working in UI but **cannot persist** due to missing API
  - Constraint editor implemented but **cannot save** due to missing API
  - Undo/Redo implemented but **limited** without full CRUD support
- 🔄 **Phase 4**: Response Collection (40% COMPLETE - BLOCKED)
  - Basic response form working
  - **Cannot retrieve answers for analysis** - missing endpoints
  - **Cannot filter/search responses** - missing endpoints
- ⏸️ **Phase 5**: Response Analysis (BLOCKED - ALL ENDPOINTS MISSING)
- ⏸️ **Phase 6**: Polish & Optimization (PENDING)

---

## 🚨 CRITICAL PRIORITY (BLOCKING DEVELOPMENT)

### 1. Question Management APIs

#### 1.1 Update Question (PUT)
**Current Issue**: Edit question works in frontend but changes are **LOCAL ONLY** - not persisted to backend.

**Endpoint**: `PUT /api/Question/{ConnectionId}/{id}`

**Request Body**:
```json
{
  "id": "guid",
  "text": "Updated question text",
  "helpText": "Updated help text",
  "order": 1,
  "questionType": "TEXT",
  "dotNetType": "System.String",
  "questionaryId": "guid",
  "constraints": [
    {
      "id": "guid",
      "order": 1,
      "policyId": "guid",
      "questionId": "guid",
      "policyRecords": [
        {
          "id": "guid",
          "value": "required:true",
          "ordem": 1,
          "constraintId": "guid"
        }
      ]
    }
  ]
}
```

**Response**: `QuestionDto` (updated question)

**Use Cases**:
- User edits question text/help text
- User changes question type
- User modifies validation constraints
- Undo/Redo operations

---

#### 1.2 Bulk Update Question Order (PATCH)
**Current Issue**: Drag-drop reordering works in UI but **order changes are not persisted**.

**Endpoint**: `PATCH /api/Question/{ConnectionId}/reorder`

**Request Body**:
```json
{
  "questionaryId": "guid",
  "questionOrders": [
    { "questionId": "guid-1", "newOrder": 1 },
    { "questionId": "guid-2", "newOrder": 2 },
    { "questionId": "guid-3", "newOrder": 3 }
  ]
}
```

**Response**: `200 OK` or updated `List<QuestionDto>`

**Use Cases**:
- User drags questions to reorder them
- Bulk operations for performance
- Maintaining question sequence

**Performance Requirement**: Should handle 100+ questions efficiently in a single call.

---

#### 1.3 Delete Question (DELETE)
**Current Issue**: Delete endpoint **not documented** in OpenAPI spec - may not exist.

**Endpoint**: `DELETE /api/Question/{ConnectionId}/{id}`

**Response**: `204 No Content`

**Use Cases**:
- User deletes a question
- Undo/Redo delete operations
- Cleanup operations

**Business Rule**: Should soft-delete if answer sessions exist for this question, hard-delete otherwise.

---

### 2. Constraint Management APIs

#### 2.1 Create Constraint (POST)
**Current Issue**: Constraint editor implemented in frontend but **cannot save constraints**.

**Endpoint**: `POST /api/Constraint/{ConnectionId}`

**Request Body**:
```json
{
  "order": 1,
  "policyId": "guid",
  "questionId": "guid",
  "policyRecords": [
    {
      "value": "required:true",
      "ordem": 1
    },
    {
      "value": "minlength:5",
      "ordem": 2
    }
  ]
}
```

**Response**: `ConstraintDto` (created constraint with ID)

---

#### 2.2 Update Constraint (PUT)
**Endpoint**: `PUT /api/Constraint/{ConnectionId}/{id}`

**Request Body**: Same as Create

**Response**: `ConstraintDto` (updated constraint)

---

#### 2.3 Delete Constraint (DELETE)
**Endpoint**: `DELETE /api/Constraint/{ConnectionId}/{id}`

**Response**: `204 No Content`

---

#### 2.4 Get Constraints by Question (GET)
**Endpoint**: `GET /api/Constraint/{ConnectionId}/question/{questionId}`

**Response**: `List<ConstraintDto>`

---

### 3. Answer Retrieval & Management APIs

#### 3.1 Get Answers by Questionary (GET)
**Current Issue**: Endpoint **not documented** - frontend needs this for Phase 5 (Response Analysis).

**Endpoint**: `GET /api/Answer/{ConnectionId}/questionary/{questionaryId}`

**Query Parameters**:
- `status` (optional): Filter by AnswerStatus (UNFINISHED, PENDING, COMPLETED, CANCELLED)
- `fromDate` (optional): Filter by creation date (ISO 8601)
- `toDate` (optional): Filter by creation date (ISO 8601)
- `user` (optional): Filter by user email
- `pageNumber` (optional, default: 1): Page number for pagination
- `pageSize` (optional, default: 50): Items per page

**Response**:
```json
{
  "items": [
    {
      "id": "guid",
      "questionaryId": "guid",
      "user": "user@example.com",
      "cardId": 12345,
      "answerStatus": {
        "answerStatus": "PENDING",
        "answerId": "guid"
      },
      "createdAt": "2026-01-28T10:00:00Z",
      "updatedAt": "2026-01-28T11:30:00Z"
    }
  ],
  "totalCount": 150,
  "pageNumber": 1,
  "pageSize": 50,
  "totalPages": 3
}
```

**Use Cases**:
- Response Analysis view (Phase 5)
- Admin dashboard showing all responses
- Filtering by status/date/user
- Pagination for performance

---

#### 3.2 Get Answer Statistics (GET)
**Endpoint**: `GET /api/Answer/{ConnectionId}/questionary/{questionaryId}/statistics`

**Response**:
```json
{
  "questionaryId": "guid",
  "totalAnswers": 150,
  "statusBreakdown": {
    "unfinished": 45,
    "pending": 30,
    "completed": 70,
    "cancelled": 5
  },
  "completionRate": 46.67,
  "averageCompletionTimeMinutes": 12.5,
  "responsesLast24Hours": 15,
  "responsesLast7Days": 89
}
```

**Use Cases**:
- Dashboard metrics
- Survey performance monitoring
- Analytics (Phase 5)

---

#### 3.3 Update Answer Metadata (PATCH)
**Endpoint**: `PATCH /api/Answer/{ConnectionId}/{id}/metadata`

**Request Body**:
```json
{
  "metadata": {
    "startTime": "2026-01-28T10:00:00Z",
    "endTime": "2026-01-28T10:15:00Z",
    "totalTimeSeconds": 900,
    "deviceInfo": "Windows 11, Chrome 120",
    "ipAddress": "192.168.1.100",
    "userAgent": "Mozilla/5.0..."
  }
}
```

**Response**: `AnswerDto`

**Use Cases**:
- Session tracking (Phase 4)
- Analytics and reporting
- Fraud detection

---

### 4. Question Response Analysis APIs

#### 4.1 Get Responses by Question (GET)
**Endpoint**: `GET /api/QuestionResponse/{ConnectionId}/question/{questionId}`

**Query Parameters**:
- `answerStatus` (optional): Filter by answer status
- `groupBy` (optional): Group responses (e.g., "value" for aggregation)

**Response**:
```json
{
  "questionId": "guid",
  "questionText": "How satisfied are you?",
  "totalResponses": 100,
  "responses": [
    {
      "value": "Very Satisfied",
      "count": 45,
      "percentage": 45.0
    },
    {
      "value": "Satisfied",
      "count": 30,
      "percentage": 30.0
    },
    {
      "value": "Neutral",
      "count": 15,
      "percentage": 15.0
    }
  ]
}
```

**Use Cases**:
- Question-level analytics
- Response distribution charts
- Export to CSV/Excel

---

#### 4.2 Get Responses by Answer (Existing - Verify Implementation)
**Endpoint**: `GET /api/QuestionResponse/{ConnectionId}/answer/{answerId}`

**Status**: Documented but **verify it returns complete data** including question text, order, type.

**Required Response Structure**:
```json
[
  {
    "id": "guid",
    "questionResponseId": "guid",
    "answerId": "guid",
    "questionId": "guid",
    "question": {
      "id": "guid",
      "text": "What is your name?",
      "questionType": "TEXT",
      "order": 1
    },
    "value": "John Doe",
    "metadata": "{\"timestamp\":\"2026-01-28T10:05:00Z\"}",
    "createdAt": "2026-01-28T10:05:00Z"
  }
]
```

---

## 📊 HIGH PRIORITY (PHASE 5 - RESPONSE ANALYSIS)

### 5. Advanced Search & Filtering

#### 5.1 Search Answers (POST)
**Endpoint**: `POST /api/Answer/{ConnectionId}/search`

**Request Body**:
```json
{
  "questionaryId": "guid",
  "filters": {
    "status": ["COMPLETED", "PENDING"],
    "userEmail": "partial-match-text",
    "dateRange": {
      "from": "2026-01-01T00:00:00Z",
      "to": "2026-01-31T23:59:59Z"
    },
    "cardIdRange": {
      "min": 1000,
      "max": 9999
    }
  },
  "sorting": {
    "field": "createdAt",
    "direction": "desc"
  },
  "pagination": {
    "pageNumber": 1,
    "pageSize": 25
  }
}
```

**Response**: Paginated list of `AnswerDto`

---

#### 5.2 Search Question Responses (POST)
**Endpoint**: `POST /api/QuestionResponse/{ConnectionId}/search`

**Request Body**:
```json
{
  "questionaryId": "guid",
  "questionId": "guid",
  "filters": {
    "valueContains": "search-text",
    "answerStatus": ["COMPLETED"],
    "dateRange": {
      "from": "2026-01-01",
      "to": "2026-01-31"
    }
  },
  "pagination": {
    "pageNumber": 1,
    "pageSize": 100
  }
}
```

**Response**: Paginated list of `QuestionResponseDto`

---

### 6. Export & Reporting APIs

#### 6.1 Export Answers to CSV (GET)
**Endpoint**: `GET /api/Answer/{ConnectionId}/questionary/{questionaryId}/export/csv`

**Query Parameters**:
- `status` (optional): Filter by status
- `fromDate`, `toDate` (optional): Date range

**Response**: `text/csv` file download

**CSV Format**:
```csv
AnswerId,User,CardId,Status,CreatedAt,CompletedAt,TotalTimeSeconds
guid-1,user1@example.com,12345,COMPLETED,2026-01-28T10:00:00Z,2026-01-28T10:15:00Z,900
guid-2,user2@example.com,12346,PENDING,2026-01-28T11:00:00Z,null,null
```

---

#### 6.2 Export Question Responses to CSV (GET)
**Endpoint**: `GET /api/QuestionResponse/{ConnectionId}/questionary/{questionaryId}/export/csv`

**Response**: `text/csv` file download

**CSV Format**:
```csv
AnswerId,User,QuestionText,QuestionType,ResponseValue,RespondedAt
guid-1,user1@example.com,"What is your name?",TEXT,"John Doe",2026-01-28T10:05:00Z
guid-1,user1@example.com,"How old are you?",INTEGER,"25",2026-01-28T10:05:30Z
```

---

#### 6.3 Generate Answer Report (GET)
**Endpoint**: `GET /api/Answer/{ConnectionId}/questionary/{questionaryId}/report`

**Query Parameters**:
- `format`: `json`, `pdf`, `excel`

**Response** (JSON format):
```json
{
  "questionaryId": "guid",
  "questionaryName": "Customer Satisfaction Survey",
  "reportGeneratedAt": "2026-01-28T15:00:00Z",
  "totalResponses": 150,
  "statistics": {
    "completionRate": 46.67,
    "averageTimeMinutes": 12.5,
    "statusBreakdown": { ... }
  },
  "questionAnalytics": [
    {
      "questionId": "guid",
      "questionText": "How satisfied are you?",
      "questionType": "RATING",
      "totalResponses": 150,
      "statistics": {
        "average": 4.2,
        "median": 4,
        "mode": 5,
        "standardDeviation": 0.8
      },
      "distribution": [
        { "value": "5", "count": 70, "percentage": 46.67 },
        { "value": "4", "count": 40, "percentage": 26.67 }
      ]
    }
  ]
}
```

---

## 🔧 MEDIUM PRIORITY (OPTIMIZATION & FEATURES)

### 7. Batch Operations

#### 7.1 Batch Create Questions (POST)
**Endpoint**: `POST /api/Question/{ConnectionId}/batch`

**Request Body**:
```json
{
  "questionaryId": "guid",
  "questions": [
    {
      "text": "Question 1",
      "questionType": "TEXT",
      "order": 1,
      "constraints": []
    },
    {
      "text": "Question 2",
      "questionType": "BOOLEAN",
      "order": 2,
      "constraints": []
    }
  ]
}
```

**Response**: `List<QuestionDto>` (all created questions)

**Use Cases**:
- Import questions from template
- Duplicate questionary
- Performance optimization

---

#### 7.2 Batch Delete Questions (DELETE)
**Endpoint**: `DELETE /api/Question/{ConnectionId}/batch`

**Request Body**:
```json
{
  "questionIds": ["guid-1", "guid-2", "guid-3"]
}
```

**Response**: `200 OK` with count of deleted questions

---

### 8. Questionary Templates & Duplication

#### 8.1 Duplicate Questionary (POST)
**Endpoint**: `POST /api/Questionary/{ConnectionId}/{id}/duplicate`

**Request Body**:
```json
{
  "newName": "Copy of Customer Satisfaction Survey",
  "includeConstraints": true,
  "includeQuestions": true
}
```

**Response**: `FullQuestionaryDto` (complete copy with new IDs)

**Use Cases**:
- Creating similar surveys
- Template-based survey creation
- A/B testing surveys

---

### 9. Answer State History

#### 9.1 Get Answer State History (GET)
**Endpoint**: `GET /api/Answer/{ConnectionId}/{id}/history`

**Response**:
```json
{
  "answerId": "guid",
  "transitions": [
    {
      "id": "guid",
      "fromState": null,
      "toState": "UNFINISHED",
      "trigger": "Created",
      "timestamp": "2026-01-28T10:00:00Z",
      "user": "user@example.com",
      "notes": "Survey started"
    },
    {
      "id": "guid",
      "fromState": "UNFINISHED",
      "toState": "PENDING",
      "trigger": "Complete",
      "timestamp": "2026-01-28T10:15:00Z",
      "user": "user@example.com",
      "notes": "Survey submitted"
    },
    {
      "id": "guid",
      "fromState": "PENDING",
      "toState": "COMPLETED",
      "trigger": "Approve",
      "timestamp": "2026-01-28T11:00:00Z",
      "user": "admin@example.com",
      "notes": "Approved by administrator"
    }
  ]
}
```

**Use Cases**:
- Audit trail
- Compliance reporting
- Debugging state issues

---

## 🛡️ SECURITY & VALIDATION REQUIREMENTS

### Authentication & Authorization
1. **Authentication**: Implement JWT bearer token authentication (currently not required in dev)
2. **Authorization**: Role-based access control (Admin, User, Viewer)
3. **Rate Limiting**: Max 100 requests/minute per user
4. **Input Validation**: Validate all inputs (max lengths, allowed characters, SQL injection prevention)

### Data Validation Rules
1. **Question Text**: 5-1000 characters, required
2. **Questionary Name**: 3-200 characters, required, unique per connection
3. **User Email**: Valid email format
4. **Card ID**: Positive integer
5. **Order**: Positive integer, unique within questionary
6. **Policy Record Value**: 1-500 characters, format: "key:value"

### Response Codes
- `200 OK`: Successful GET/PUT/PATCH
- `201 Created`: Successful POST
- `204 No Content`: Successful DELETE
- `400 Bad Request`: Validation error (return detailed error messages)
- `404 Not Found`: Resource not found
- `409 Conflict`: Duplicate resource (e.g., duplicate question order)
- `500 Internal Server Error`: Server error (log stack trace, return generic message)

---

## 📈 PERFORMANCE REQUIREMENTS

### Response Time Targets
- **Simple GET** (by ID): < 100ms
- **List GET** (with filters): < 500ms
- **POST/PUT/DELETE**: < 300ms
- **Batch Operations**: < 1000ms for 100 items
- **Export (CSV)**: < 3000ms for 1000 records
- **Statistics/Reports**: < 2000ms

### Scalability
- Support **10,000+ questions** per questionary
- Support **100,000+ answer sessions** per questionary
- Pagination for all list endpoints (max 100 items per page)
- Lazy loading for related entities

### Caching Strategy
- Cache questionary metadata (5 minutes)
- Cache question types and policies (1 hour)
- Invalidate cache on updates
- ETags for conditional requests

---

## 🧪 TESTING REQUIREMENTS

### Unit Tests
- All business logic covered (min 80% coverage)
- Validation rules tested
- Edge cases tested (empty lists, null values, max lengths)

### Integration Tests
- All endpoints tested with sample data
- Authentication/authorization tested
- Error handling tested
- Database constraints tested

### Load Tests
- 100 concurrent users
- 1000 requests/second
- Response time degradation < 10% under load

---

## 📝 API DOCUMENTATION REQUIREMENTS

### OpenAPI 3.0 Specification
- All new endpoints documented
- Request/response examples included
- Error responses documented
- Authentication requirements specified

### Changelog
- Version all API changes
- Document breaking changes
- Provide migration guides

---

## 🚀 IMPLEMENTATION PRIORITY

### Phase 1: CRITICAL (Week 1) - BLOCKS FRONTEND
1. ✅ Question Update (PUT)
2. ✅ Question Delete (DELETE)
3. ✅ Bulk Question Reorder (PATCH)
4. ✅ Constraint CRUD (POST, PUT, DELETE, GET)

### Phase 2: HIGH (Week 2) - ENABLES PHASE 5
5. ✅ Get Answers by Questionary (GET with filters)
6. ✅ Get Answer Statistics (GET)
7. ✅ Get Responses by Question (GET with aggregation)
8. ✅ Answer Search (POST)

### Phase 3: MEDIUM (Week 3) - FEATURES
9. ✅ Export to CSV (Answers & Responses)
10. ✅ Generate Report (GET)
11. ✅ Batch Operations (Questions)
12. ✅ Duplicate Questionary (POST)
13. ✅ Answer State History (GET)

### Phase 4: LOW (Week 4) - OPTIMIZATION
14. Performance optimization
15. Advanced caching
16. Load testing and tuning

---

## 📞 CONTACT & COORDINATION

**Frontend Team Status**: Ready to integrate immediately upon API availability  
**Blocking Issues**: 13 endpoints (4 critical, 4 high priority, 5 medium priority)  
**Frontend Build**: Passing (0 errors, 0 warnings)  
**Integration Tests**: Ready to run once APIs deployed

### Integration Testing Plan
1. Backend deploys new endpoints to **http://localhost:5049/api/**
2. Frontend updates connection configuration (already using port 5049)
3. Frontend runs integration tests against local backend
4. Joint testing session for edge cases
5. Performance testing with realistic data volumes
6. Production deployment coordination

---

## ✅ SUCCESS CRITERIA

### Definition of Done
- [ ] All endpoints implemented per specification
- [ ] Unit tests passing (>80% coverage)
- [ ] Integration tests passing
- [ ] OpenAPI documentation updated
- [ ] Performance benchmarks met
- [ ] Security review passed
- [ ] Frontend integration successful
- [ ] Load testing passed (100 concurrent users)

### Acceptance Testing
Frontend team will verify:
1. Question edit/delete operations persist correctly
2. Drag-drop reordering saves to database
3. Constraints save and load properly
4. Response analysis views populate correctly
5. Export functionality produces valid CSV files
6. Search and filtering return accurate results
7. Statistics calculations are correct
8. Error handling displays user-friendly messages

---

## 📎 APPENDIX

### Related Documents
- `/API_DOCUMENTATION.md` - Current API endpoints
- `/FRONTEND_TECHNICAL_DOCUMENTATION.md` - Frontend specifications
- `/PHASE3_IMPLEMENTATION_SUMMARY.md` - Frontend Phase 3 status
- `/PHASE4_IMPLEMENTATION_SUMMARY.md` - Frontend Phase 4 status
- `/CONSTRAINT_EDITOR_IMPLEMENTATION.md` - Constraint editor details

### OpenAPI Specification Updates Needed
Please update the OpenAPI spec at your backend repository to include all new endpoints with:
- Full request/response schemas
- Example payloads
- Error responses
- Authentication requirements

### Database Schema Considerations
Ensure the following tables support the new functionality:
- `Questions` table: Needs `Order` column with unique constraint per QuestionaryId
- `Constraints` table: Proper foreign keys to Questions and Policies
- `Answers` table: Metadata column (JSONB/TEXT) for flexible data storage
- `StateHistory` table (new?): For tracking answer state transitions
- Indexes on frequently queried columns (QuestionaryId, AnswerId, Status, CreatedAt)

---

**Document Version**: 1.0  
**Last Updated**: January 28, 2026  
**Next Review**: Upon backend implementation completion  
**Questions/Clarifications**: Contact frontend team via project Slack/Teams channel

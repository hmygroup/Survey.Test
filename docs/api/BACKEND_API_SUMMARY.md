# Backend API Requirements - Quick Summary

**Date**: January 28, 2026  
**Priority**: HIGH - Frontend development blocked  
**Full Details**: See [BACKEND_API_REQUIREMENTS.md](./BACKEND_API_REQUIREMENTS.md)

---

## 🚨 Situation

The WPF frontend application has completed **Phase 1-3 (100%)** and **Phase 4 (40%)**, but further development is **BLOCKED** by missing backend APIs. Several frontend features are implemented but **cannot persist data** or **cannot function** without backend support.

---

## ❌ Missing Critical APIs (Week 1 Required)

### 1. Question Management
```http
PUT    /api/Question/{ConnectionId}/{id}           # Update question
DELETE /api/Question/{ConnectionId}/{id}           # Delete question  
PATCH  /api/Question/{ConnectionId}/reorder        # Bulk reorder questions
```

**Impact**: 
- Edit question works in UI but changes are **LOCAL ONLY**
- Drag-drop reordering works but **ORDER NOT SAVED**
- Delete may not work (endpoint not documented)

---

### 2. Constraint Management
```http
POST   /api/Constraint/{ConnectionId}              # Create constraint
PUT    /api/Constraint/{ConnectionId}/{id}         # Update constraint
DELETE /api/Constraint/{ConnectionId}/{id}         # Delete constraint
GET    /api/Constraint/{ConnectionId}/question/{questionId}  # Get by question
```

**Impact**: 
- Constraint editor fully implemented but **CANNOT SAVE**
- Validation rules cannot be persisted

---

## ⚠️ Missing High Priority APIs (Week 2 Required for Phase 5)

### 3. Answer Retrieval & Analysis
```http
GET    /api/Answer/{ConnectionId}/questionary/{questionaryId}  # Get with filters
GET    /api/Answer/{ConnectionId}/questionary/{questionaryId}/statistics
POST   /api/Answer/{ConnectionId}/search                       # Advanced search
GET    /api/QuestionResponse/{ConnectionId}/question/{questionId}  # Aggregation
```

**Impact**: 
- **Phase 5 (Response Analysis) completely blocked**
- Cannot view/analyze survey responses
- No dashboard metrics available

---

## 📊 Missing Medium Priority APIs (Week 3 - Nice to Have)

```http
GET    /api/Answer/{ConnectionId}/questionary/{questionaryId}/export/csv
GET    /api/QuestionResponse/{ConnectionId}/questionary/{questionaryId}/export/csv
POST   /api/Question/{ConnectionId}/batch
POST   /api/Questionary/{ConnectionId}/{id}/duplicate
GET    /api/Answer/{ConnectionId}/{id}/history
PATCH  /api/Answer/{ConnectionId}/{id}/metadata
```

**Impact**: 
- Export functionality not available
- Performance optimization limited
- Enhanced features delayed

---

## 📋 API Specifications

### Example: Update Question (PUT)

**Endpoint**: `PUT /api/Question/{ConnectionId}/{id}`

**Request**:
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "text": "Updated question text",
  "helpText": "Updated help text",
  "order": 1,
  "questionType": "TEXT",
  "dotNetType": "System.String",
  "questionaryId": "650e8400-e29b-41d4-a716-446655440001",
  "constraints": [
    {
      "order": 1,
      "policyId": "guid",
      "policyRecords": [
        { "value": "required:true", "ordem": 1 },
        { "value": "minlength:5", "ordem": 2 }
      ]
    }
  ]
}
```

**Response**: `200 OK` with updated `QuestionDto`

---

### Example: Bulk Reorder Questions (PATCH)

**Endpoint**: `PATCH /api/Question/{ConnectionId}/reorder`

**Request**:
```json
{
  "questionaryId": "650e8400-e29b-41d4-a716-446655440001",
  "questionOrders": [
    { "questionId": "550e8400-e29b-41d4-a716-446655440000", "newOrder": 3 },
    { "questionId": "550e8400-e29b-41d4-a716-446655440002", "newOrder": 1 },
    { "questionId": "550e8400-e29b-41d4-a716-446655440003", "newOrder": 2 }
  ]
}
```

**Response**: `200 OK`

---

### Example: Get Answers by Questionary (GET)

**Endpoint**: `GET /api/Answer/{ConnectionId}/questionary/{questionaryId}`

**Query Parameters**:
- `status` (optional): UNFINISHED, PENDING, COMPLETED, CANCELLED
- `fromDate`, `toDate` (optional): ISO 8601 dates
- `user` (optional): Filter by email
- `pageNumber`, `pageSize` (optional): Pagination

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
      "createdAt": "2026-01-28T10:00:00Z"
    }
  ],
  "totalCount": 150,
  "pageNumber": 1,
  "pageSize": 50,
  "totalPages": 3
}
```

---

## 🎯 Implementation Priority

### Week 1: CRITICAL (Unblocks Phase 3)
1. Question Update (PUT)
2. Question Delete (DELETE)
3. Bulk Question Reorder (PATCH)
4. Constraint CRUD (POST, PUT, DELETE, GET)

**Deliverable**: Frontend can fully persist all question/constraint changes

---

### Week 2: HIGH (Enables Phase 5)
5. Get Answers by Questionary (GET with filters & pagination)
6. Get Answer Statistics (GET)
7. Get Responses by Question (GET with aggregation)
8. Answer Search (POST)

**Deliverable**: Frontend can build Response Analysis views

---

### Week 3: MEDIUM (Enhanced Features)
9. Export to CSV (Answers & Responses)
10. Batch Operations
11. Duplicate Questionary
12. Answer History & Metadata

**Deliverable**: Full feature parity and performance optimization

---

## 🔧 Technical Requirements

### Response Codes
- `200 OK`: Successful operation
- `201 Created`: Resource created
- `204 No Content`: Successful delete
- `400 Bad Request`: Validation error (return detailed message)
- `404 Not Found`: Resource not found
- `409 Conflict`: Duplicate resource

### Performance
- Simple GET: < 100ms
- List GET: < 500ms
- POST/PUT/DELETE: < 300ms
- Batch operations: < 1000ms for 100 items

### Validation
- Question text: 5-1000 characters, required
- Questionary name: 3-200 characters, unique
- Order: Positive integer, unique within questionary
- Input sanitization for SQL injection prevention

### Pagination
- Default page size: 50
- Max page size: 100
- Return totalCount, pageNumber, pageSize, totalPages

---

## 📞 Coordination

### Frontend Status
- ✅ Implemented and ready to integrate
- ✅ Build passing (0 errors, 0 warnings)
- ✅ Integration tests prepared
- ⏸️ Waiting for backend APIs

### Integration Plan
1. Backend deploys endpoints to `http://localhost:5049/api/`
2. Backend updates OpenAPI specification
3. Frontend runs integration tests
4. Joint testing session
5. Performance tuning
6. Production deployment

### Communication
- **Blocking Issues**: 13 missing endpoints
- **Questions**: Contact frontend team via Slack/Teams
- **Full Spec**: See [BACKEND_API_REQUIREMENTS.md](./BACKEND_API_REQUIREMENTS.md)

---

## ✅ Success Criteria

- [ ] All critical endpoints implemented (Week 1)
- [ ] Unit tests passing (>80% coverage)
- [ ] OpenAPI spec updated
- [ ] Frontend integration successful
- [ ] Performance benchmarks met
- [ ] Security review passed

---

**Next Action**: Backend team reviews full specification in [BACKEND_API_REQUIREMENTS.md](./BACKEND_API_REQUIREMENTS.md)

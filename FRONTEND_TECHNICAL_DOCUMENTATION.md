# 📋 Documentación Técnica - Frontend de Gestión de Cuestionarios
## WPF + WinUI - Gestión de Encuestas/Cuestionarios

**Versión:** 1.0  
**Fecha:** Enero 2026  
**Framework:** WPF + WinUI  
**Patrón:** MVVM + CQRS (Backend)  
**API Base:** `http://localhost:5030/api/`

---

## 📑 Tabla de Contenidos

1. [Introducción y Objetivos](#introducción-y-objetivos)
2. [Análisis del Backend](#análisis-del-backend)
3. [Arquitectura del Frontend](#arquitectura-del-frontend)
4. [Reglas de UI/UX](#reglas-de-uiux)
5. [Patrones de Implementación](#patrones-de-implementación)
6. [Endpoints Disponibles](#endpoints-disponibles)
7. [Estructura de Datos (DTOs)](#estructura-de-datos-dtos)
8. [Flujos de Negocio](#flujos-de-negocio)
9. [Guía de Estilo Visual](#guía-de-estilo-visual)
10. [Checklist de Implementación](#checklist-de-implementación)

---

## Introducción y Objetivos

### 🎯 Objetivo Principal
Crear una **aplicación WPF moderna** con **WinUI** que permita gestionar cuestionarios de manera intuitiva, similar a **Google Forms**, pero con funcionalidades empresariales avanzadas.

### ✨ Características Principales
- **Crear cuestionarios** con editor visual intuitivo
- **Gestionar preguntas** con múltiples tipos de respuesta
- **Aplicar restricciones y validaciones** mediante políticas
- **Recopilar respuestas** y hacer seguimiento
- **Exportar datos** para análisis
- **Interfaz responsiva** y accesible
- **Soporte multiidioma** (inglés/español)

### 📱 Plataforma
- **Framework:** WPF (Windows Presentation Foundation)
- **Librería Moderna:** WinUI 3
- **Patrón Arquitectónico:** MVVM (Model-View-ViewModel)
- **Validación Backend:** CQRS + MediatR

---

## Nuances y Funcionamiento Interno del Sistema

### 🔍 Conceptos Fundamentales

El sistema de cuestionarios se basa en una separación clara entre **definición** y **respuesta**:

```
DEFINICIÓN (Questionary + Questions)
    ↓
SESIÓN DE RESPUESTA (Answer)
    ↓
RESPUESTAS INDIVIDUALES (QuestionResponses)
```

### 🧩 Arquitectura de Respuestas: Answer vs QuestionResponse

Esta es una de las partes más importantes para entender:

#### **Answer** (Respuesta Global)
- Representa **UNA SESIÓN** de usuario respondiendo el cuestionario
- Es como "abrir" el cuestionario para llenarlo
- Contiene metadatos: quién responde (`user`), cuándo, qué tarjeta (`cardId`)
- Tiene un **estado** que evoluciona: `UNFINISHED` → `PENDING` → `COMPLETED`
- **Un usuario puede tener múltiples Answers** del mismo cuestionario (múltiples intentos)

```csharp
// Ejemplo: Juan abre el cuestionario de satisfacción
Answer {
    Id: "123e4567-e89b-12d3-a456-426614174000",
    QuestionaryId: "550e8400-e29b-41d4-a716-446655440000",
    User: "juan@empresa.com",
    CardId: 12345,
    AnswerStatus: UNFINISHED  // ← Apenas está iniciando
}
```

#### **QuestionResponse** (Respuesta Individual)
- Representa la **respuesta a UNA pregunta específica** dentro de una sesión
- Está vinculado a un `Answer` (la sesión) y a una `Question`
- Contiene el valor real de la respuesta en `Response` (string)
- Puede tener `Metadata` adicional (JSON) para info extra

```csharp
// Juan responde la primera pregunta
QuestionResponse {
    Id: "789e4567-e89b-12d3-a456-426614174111",
    QuestionId: "pregunta-1-guid",
    AnswerId: "123e4567-e89b-12d3-a456-426614174000",  // ← Link a la sesión
    Response: "Muy satisfecho",
    Metadata: "{ \"timeSpent\": 5000, \"device\": \"desktop\" }"
}
```

### 🔄 Flujo Completo de Vida de una Respuesta

```
1. INICIO DE SESIÓN
   POST /api/answer/{connection}?questionaryId={id}&user=juan@empresa.com
   → Crea Answer con estado UNFINISHED
   → Devuelve AnswerId que se usará para todo lo demás

2. GUARDANDO RESPUESTAS (puede ser incremental)
   POST /api/questionresponse/{connection}/response
   Body: [
     { questionId: "...", response: "..." },
     { questionId: "...", response: "..." }
   ]
   → Crea/actualiza QuestionResponses vinculados al AnswerId
   → El usuario puede guardar parcialmente y continuar después

3. COMPLETAR EL CUESTIONARIO
   PUT /api/answer/setStatus
   Body: { answersId: ["123e4567..."], ANSWER_STATUS: "COMPLETED" }
   → Cambia el estado del Answer a COMPLETED
   → Marca la sesión como finalizada

4. (OPCIONAL) MODIFICAR RESPUESTA INDIVIDUAL
   PATCH /api/questionresponse/{connection}/response?
     questionResponseId={id}&
     response=nuevo_valor
   → Actualiza una respuesta específica sin afectar las demás
```

### 🎯 Parámetro Connection: ¿Qué es y por qué es crucial?

El parámetro `connection` es un **identificador de base de datos multi-tenant**:

**Escenario típico:**
```
Empresa A → connection: 1 → Base de datos: BBDD_EmpresaA
Empresa B → connection: 2 → Base de datos: BBDD_EmpresaB
Empresa C → connection: 3 → Base de datos: BBDD_EmpresaC
```

**Implicaciones para el frontend:**
- ✅ **SIEMPRE** debe estar presente en casi todas las llamadas API
- ✅ Normalmente viene de la sesión del usuario (login)
- ✅ Debe ser consistente durante toda la sesión
- ✅ Si cambias de `connection`, estás cambiando de "empresa" o "contexto"
- ⚠️ **Nunca hardcodear** - debe ser dinámico

```csharp
// Mal ❌
var result = await apiService.GetQuestionnairesAsync(1); 

// Bien ✅
var connectionId = SessionManager.CurrentConnectionId;
var result = await apiService.GetQuestionnairesAsync(connectionId);
```

### 🔐 Sistema de Constraints y Policies: Validación Avanzada

Este es uno de los aspectos más sofisticados del sistema.

#### Jerarquía de Validación

```
Question (Pregunta)
    ↓
Constraint (Restricción aplicada a la pregunta)
    ↓
Policy (Tipo de validación: email, teléfono, rango, etc.)
    ↓
PolicyRecords (Valores o parámetros de la política)
```

#### Ejemplo Práctico 1: Validación de Email

```json
{
  "questionText": "¿Cuál es su correo electrónico?",
  "constraints": [
    {
      "policy": {
        "id": "550e8400-...",
        "name": "Email Validation"
      },
      "policyRecords": [
        {
          "value": "pattern:^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$"
        },
        {
          "value": "errorMessage:Ingrese un email válido"
        }
      ]
    }
  ]
}
```

#### Ejemplo Práctico 2: Rango Numérico

```json
{
  "questionText": "¿Cuál es su edad?",
  "constraints": [
    {
      "policy": {
        "name": "Numeric Range"
      },
      "policyRecords": [
        { "value": "min:18" },
        { "value": "max:99" },
        { "value": "errorMessage:Debe tener entre 18 y 99 años" }
      ]
    }
  ]
}
```

#### Ejemplo Práctico 3: Opciones Permitidas

```json
{
  "questionText": "Seleccione su departamento",
  "constraints": [
    {
      "policy": {
        "name": "Allowed Values"
      },
      "policyRecords": [
        { "value": "option:Ventas" },
        { "value": "option:Marketing" },
        { "value": "option:IT" },
        { "value": "option:RRHH" }
      ]
    }
  ]
}
```

**Cómo interpretarlo en el frontend:**
1. Obtén las políticas disponibles: `GET /api/policy/{connection}/all`
2. Al crear una pregunta, el usuario selecciona una o varias políticas
3. Según la política, muestra campos para configurar `PolicyRecords`
4. Al guardar, incluyes todo en el `QuestionCreationDto`
5. **Cuando el usuario responde**, validas contra estos constraints en tiempo real

### 🏗️ QuestionType: Tipos de Datos y su Interpretación

El sistema usa tipos de .NET para definir qué tipo de respuesta espera cada pregunta:

| DotNetType | Tipo de UI | Validación |
|-----------|-----------|-----------|
| `System.String` | TextBox / TextArea | Máx. caracteres, pattern |
| `System.Boolean` | CheckBox / Switch | true/false |
| `System.Int32` | NumericUpDown | Rango numérico |
| `System.DateTime` | DatePicker | Rango de fechas |
| `Hmy.Web.Common.Services.Survey.Domain.CustomDatatypes.Attachment` | File Upload | Tipo de archivo, tamaño |

**Ejemplo de lógica en el frontend:**

```csharp
private UIElement CreateQuestionControl(QuestionDto question)
{
    switch (question.QuestionType.DotNetType)
    {
        case "System.String":
            return new TextBox 
            { 
                PlaceholderText = "Ingrese su respuesta...",
                MaxLength = GetMaxLengthFromConstraints(question.Constraints)
            };
            
        case "System.Boolean":
            return new CheckBox 
            { 
                Content = question.QuestionText 
            };
            
        case "System.Int32":
            return new NumberBox 
            { 
                Minimum = GetMinFromConstraints(question.Constraints),
                Maximum = GetMaxFromConstraints(question.Constraints)
            };
            
        case "System.DateTime":
            return new DatePicker 
            { 
                MinDate = GetMinDateFromConstraints(question.Constraints),
                MaxDate = GetMaxDateFromConstraints(question.Constraints)
            };
            
        case "Hmy.Web.Common.Services.Survey.Domain.CustomDatatypes.Attachment":
            return new FileUploadControl 
            { 
                AllowedExtensions = GetAllowedExtensionsFromConstraints(question.Constraints)
            };
            
        default:
            return new TextBox(); // Fallback
    }
}
```

### 📊 Metadata: Información Contextual Adicional

El campo `Metadata` en `QuestionResponse` es un **JSON libre** que permite guardar información extra:

**Casos de uso:**
```json
{
  "timeSpent": 12500,           // Milisegundos que tardó en responder
  "attempts": 3,                // Número de veces que cambió la respuesta
  "device": "mobile",           // Dispositivo usado
  "browser": "Chrome 120",      // Navegador
  "geolocation": {              // Ubicación (si tiene permiso)
    "lat": 40.7128,
    "lng": -74.0060
  },
  "confidence": 0.85,           // Nivel de confianza en la respuesta
  "skipped": false,             // Si saltó la pregunta inicialmente
  "lastModified": "2026-01-28T15:30:00Z"
}
```

**En el frontend:**
```csharp
// Al crear la respuesta, puedes agregar metadata
var response = new CreateResponseBody
{
    QuestionId = question.Id,
    Response = userAnswer,
    Metadata = JsonConvert.SerializeObject(new
    {
        timeSpent = stopwatch.ElapsedMilliseconds,
        device = DeviceInfo.DeviceType,
        browser = BrowserInfo.Name,
        attempts = answerAttempts
    })
};
```

### 🔄 Estados del Answer: Máquina de Estados

Los estados del `Answer` siguen una progresión lógica:

```
INICIO
  ↓
UNFINISHED (No terminado)
  ↓ (usuario completa todas las preguntas)
PENDING (Pendiente de validación/revisión)
  ↓ (administrador/sistema valida)
COMPLETED (Completado y validado)

        ↓ (en cualquier momento)
      CANCELLED (Cancelado)
```

**Reglas de transición:**
- `UNFINISHED → PENDING`: Cuando el usuario termina de responder
- `PENDING → COMPLETED`: Cuando se valida/aprueba la respuesta
- `PENDING → UNFINISHED`: Si se requieren correcciones
- `Cualquiera → CANCELLED`: Usuario o sistema cancela

**Validaciones importantes:**
- ❌ No puedes cambiar de `COMPLETED` a `UNFINISHED` (requiere crear nueva sesión)
- ✅ Puedes tener múltiples `UNFINISHED` del mismo usuario (sesiones parciales)
- ✅ Filtrar por estado para mostrar solo respuestas completas/pendientes

### 🔗 Relaciones y Cardinalidad

```
Questionary (1) ←→ (N) Questions
    ↓ (1)                 ↓ (1)
    ↓                     ↓
  Answer (N)        QuestionResponse (N)
    ↓ (1)                 ↓ (N)
    └─────────────────────┘
```

**Implicaciones:**
- Un cuestionario puede tener múltiples respuestas de diferentes usuarios
- Un usuario puede tener múltiples respuestas del mismo cuestionario (intentos)
- Cada respuesta (Answer) agrupa todas las respuestas individuales (QuestionResponses)
- Una pregunta puede tener muchas respuestas de diferentes sesiones

### 🎨 FullQuestionaryDto vs QuestionaryDto: Cuándo usar cada uno

#### QuestionaryDto (Ligero)
```json
{
  "id": "...",
  "name": "Encuesta de Satisfacción"
}
```
**Usar cuando:**
- Listar cuestionarios
- Búsquedas rápidas
- Selección de cuestionario
- Performance es crítico

#### FullQuestionaryDto (Completo)
```json
{
  "id": "...",
  "name": "Encuesta de Satisfacción",
  "questions": [
    {
      "id": "...",
      "questionText": "...",
      "questionType": { ... },
      "constraints": [ ... ]
    }
  ]
}
```
**Usar cuando:**
- Editor de cuestionario
- Vista de respuesta (formulario)
- Exportación completa
- Análisis detallado

### ⚠️ Inconsistencias Conocidas del Backend

Estas son inconsistencias reales que debes manejar en el frontend:

#### 1. **Nomenclatura Inconsistente**
```csharp
// A veces camelCase
{ "connectionId": 1 }

// A veces PascalCase
{ "QuestionaryId": "..." }

// A veces lowercase con underscore
{ "answer_status": "COMPLETED" }
```

**Solución:** Usar mappers/adaptadores en el frontend para normalizar.

#### 2. **Parámetros Query vs Route**
```http
// A veces en la ruta
POST /api/questionary/{connection}/New/{name}

// A veces en query
POST /api/question/new/{connection}?questionaryId={id}

// A veces mixto
PATCH /api/questionresponse/{connection}/response?
  questionResponseId={id}&response={valor}
```

**Solución:** Documentar cada endpoint claramente y seguir la convención del backend.

#### 3. **Respuestas Unit vs Objeto**
```csharp
// Algunos endpoints no retornan nada útil
PUT /api/answer/setStatus
Response: Unit (equivalente a void)

// Otros sí retornan el objeto creado/modificado
POST /api/questionary/{connection}/New/{name}
Response: QuestionaryDto
```

**Solución:** Después de operaciones Unit, hacer un GET para refrescar datos.

### 🔍 Query vs Command Pattern (CQRS en Backend)

El backend usa **CQRS** (Command Query Responsibility Segregation):

**Queries (Consultas - GET):**
- Solo leen datos
- No modifican estado
- Pueden cachearse
- Pueden ejecutarse en paralelo

**Commands (Comandos - POST/PUT/DELETE):**
- Modifican estado
- No retornan grandes cantidades de datos
- Deben ejecutarse de forma secuencial
- Pueden fallar con validaciones

**Implicación para el frontend:**
```csharp
// ✅ Bien: Queries en paralelo
await Task.WhenAll(
    questionaryService.GetAllAsync(connectionId),
    questionTypeService.GetAllTypesAsync(connectionId),
    policyService.GetAllPoliciesAsync(connectionId)
);

// ❌ Mal: Commands en paralelo (pueden causar race conditions)
await Task.WhenAll(
    questionService.CreateAsync(...),
    questionService.CreateAsync(...)
);

// ✅ Bien: Commands secuenciales
foreach (var question in questions)
{
    await questionService.CreateAsync(question);
}
```

### 🧪 Escenarios Edge Cases a Considerar

#### 1. **Usuario responde parcialmente y cierra la app**
```
- Answer está en UNFINISHED
- Solo tiene 3 de 10 QuestionResponses
- Al reabrir, debe recuperar la sesión
- Cargar respuestas existentes
- Permitir continuar desde donde quedó
```

#### 2. **Cuestionario se modifica mientras hay respuestas en curso**
```
- Questionary tiene 5 preguntas
- Usuario A está respondiendo (UNFINISHED)
- Admin agrega una pregunta nueva
- Usuario A completa → tiene 5 respuestas, faltan 1
- ¿Cómo manejar? 
  → Opción 1: Versionar cuestionarios
  → Opción 2: Mostrar alerta de cambios
  → Opción 3: Solo permitir respuestas de la versión exacta
```

#### 3. **Múltiples respuestas del mismo usuario**
```
- Juan responde el cuestionario → Answer 1 (COMPLETED)
- Juan quiere responder de nuevo → Crear Answer 2
- Diferenciar en la UI (Intento 1, Intento 2)
- Permitir comparar respuestas
```

#### 4. **Validación asíncrona vs síncrona**
```
- Constraints locales: validar inmediatamente (pattern, range)
- Constraints remotos: validar contra API (duplicados, disponibilidad)
- UX: Mostrar validación en tiempo real para locales
- UX: Mostrar spinner para remotas
```

### 📱 Consideraciones de UX Avanzadas

#### 1. **Progreso Visible**
```csharp
// Calcular y mostrar progreso
public class ResponseProgressTracker
{
    public int TotalQuestions { get; set; }
    public int AnsweredQuestions { get; set; }
    public double ProgressPercentage => 
        (double)AnsweredQuestions / TotalQuestions * 100;
    public TimeSpan EstimatedTimeRemaining { get; set; }
    
    public string ProgressText => 
        $"{AnsweredQuestions} de {TotalQuestions} respondidas " +
        $"({ProgressPercentage:F0}%)";
}
```

#### 2. **Deshacer Cambios**
```csharp
// En el editor, permitir deshacer antes de guardar
public class QuestionaryEditor
{
    private Stack<QuestionaryState> _history = new();
    
    public void SaveSnapshot()
    {
        _history.Push(CurrentQuestionary.Clone());
    }
    
    public void Undo()
    {
        if (_history.Count > 0)
            CurrentQuestionary = _history.Pop();
    }
}
```

#### 3. **Validación Progresiva**
```
- Usuario empieza a escribir → Sin validación
- Usuario hace pausa (500ms) → Validar formato
- Usuario sale del campo (blur) → Validación completa
- Usuario intenta enviar → Validación final + backend
```

### 🎓 Patrones Recomendados

#### 1. **Repository Pattern para API**
```csharp
public interface IQuestionaryRepository
{
    Task<QuestionaryDto> GetByIdAsync(int connection, Guid id);
    Task<IEnumerable<QuestionaryDto>> GetAllAsync(int connection);
    Task<QuestionaryDto> CreateAsync(int connection, string name);
    Task<FullQuestionaryDto> GetFullAsync(int connection, Guid id);
}

// Implementación con cache
public class CachedQuestionaryRepository : IQuestionaryRepository
{
    private readonly IQuestionaryRepository _inner;
    private readonly IMemoryCache _cache;
    
    public async Task<QuestionaryDto> GetByIdAsync(int connection, Guid id)
    {
        var key = $"questionary_{connection}_{id}";
        if (_cache.TryGetValue(key, out QuestionaryDto cached))
            return cached;
            
        var result = await _inner.GetByIdAsync(connection, id);
        _cache.Set(key, result, TimeSpan.FromMinutes(5));
        return result;
    }
}
```

#### 2. **State Management**
```csharp
// Centralizar el estado de la aplicación
public class AppState
{
    public int CurrentConnectionId { get; set; }
    public UserSession CurrentUser { get; set; }
    public QuestionaryDto ActiveQuestionary { get; set; }
    public AnswerDto CurrentAnswer { get; set; }
    public List<QuestionResponseDto> CurrentResponses { get; set; }
    
    public event EventHandler StateChanged;
    
    public void UpdateState(Action<AppState> mutator)
    {
        mutator(this);
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
```

#### 3. **Validation Chain**
```csharp
public interface IValidator<T>
{
    ValidationResult Validate(T value);
}

public class ValidationChain<T>
{
    private List<IValidator<T>> _validators = new();
    
    public ValidationChain<T> Add(IValidator<T> validator)
    {
        _validators.Add(validator);
        return this;
    }
    
    public ValidationResult ValidateAll(T value)
    {
        var results = new List<ValidationResult>();
        foreach (var validator in _validators)
        {
            var result = validator.Validate(value);
            if (!result.IsValid)
                results.Add(result);
        }
        return ValidationResult.Combine(results);
    }
}

// Uso:
var chain = new ValidationChain<string>()
    .Add(new RequiredValidator())
    .Add(new MinLengthValidator(5))
    .Add(new MaxLengthValidator(500))
    .Add(new PatternValidator(@"^[a-zA-Z0-9\s]+$"));
    
var result = chain.ValidateAll(userInput);
```

---

## Análisis del Backend

### 🏗️ Arquitectura Backend

El backend implementa **Clean Architecture** con **CQRS** usando **MediatR**:

```
HMY.Web.Common.Services.Survey
├── Api/               (Controllers & Routing)
├── Application/       (Business Logic - CQRS)
├── Domain/            (Entities & Business Rules)
├── Persistence/       (Database Access)
└── Infrastructure/    (External Services)
```

### 🔗 URL Base API
```
http://localhost:5030/api/
```

### 📊 Modelo de Datos Principal

```
Questionary (Cuestionario)
├── Questions (Preguntas)
│   ├── QuestionType (Tipo: texto, opción múltiple, etc.)
│   ├── Constraints (Restricciones/Validaciones)
│   │   └── Policy (Política asociada)
│   │       └── PolicyRecords (Valores permitidos)
│   └── QuestionResponses (Respuestas enviadas)
│
└── Answers (Respuestas globales del cuestionario)
    ├── AnswerStatus (Estado: COMPLETED, UNFINISHED, PENDING, CANCELLED)
    └── QuestionResponses (Respuesta individual por pregunta)
```

### 🔑 Parámetro Connection
**CRÍTICO:** Todos los endpoints requieren el parámetro `connection` (int) que identifica la base de datos a usar.

---

## Arquitectura del Frontend

### 🏛️ Estructura de Proyecto Recomendada

```
SurveyManagementApp/
├── Views/
│   ├── MainWindow.xaml
│   ├── QuestionaryList/
│   │   └── QuestionaryListView.xaml
│   ├── QuestionaryEditor/
│   │   ├── QuestionaryEditorView.xaml
│   │   ├── QuestionEditor/
│   │   │   └── QuestionEditorView.xaml
│   │   ├── ConstraintEditor/
│   │   │   └── ConstraintEditorView.xaml
│   │   └── PreviewView.xaml
│   ├── ResponseManager/
│   │   ├── ResponseListView.xaml
│   │   └── ResponseDetailView.xaml
│   └── Settings/
│       └── SettingsView.xaml
│
├── ViewModels/
│   ├── MainWindowViewModel.cs
│   ├── QuestionaryListViewModel.cs
│   ├── QuestionaryEditorViewModel.cs
│   ├── QuestionEditorViewModel.cs
│   ├── ResponseManagerViewModel.cs
│   └── Base/
│       └── ViewModelBase.cs
│
├── Models/
│   ├── ApiClient.cs
│   ├── QuestionaryModel.cs
│   ├── QuestionModel.cs
│   ├── ResponseModel.cs
│   └── ValidationModel.cs
│
├── Services/
│   ├── ApiService.cs
│   ├── QuestionaryService.cs
│   ├── QuestionService.cs
│   ├── ResponseService.cs
│   ├── ValidationService.cs
│   └── DialogService.cs
│
├── Converters/
│   ├── EnumToStringConverter.cs
│   ├── BoolToVisibilityConverter.cs
│   └── DateToStringConverter.cs
│
├── Helpers/
│   ├── LoggingHelper.cs
│   ├── ValidationHelper.cs
│   └── ThemeHelper.cs
│
├── Resources/
│   ├── Strings/
│   │   ├── Resources.es.resx
│   │   └── Resources.en.resx
│   ├── Themes/
│   │   ├── Light.xaml
│   │   ├── Dark.xaml
│   │   └── Neutral.xaml
│   └── Icons/
│       └── [SVG files]
│
└── App.xaml
```

### 🏗️ Patrón MVVM

**ViewModel Base:**
```csharp
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    
    protected void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    
    protected bool SetProperty<T>(ref T storage, T value, string propertyName)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
            return false;
        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
```

---

## Reglas de UI/UX

### 🎨 Principios de Diseño

#### 1. **Consistencia Visual**
- ✅ Usar **WinUI Design System** como base
- ✅ Paleta de colores limitada (máximo 5 colores primarios)
- ✅ Tipografía consistente (Segoe UI, 14px body, 20px headers)
- ✅ Espaciado uniforme (múltiplos de 4px)
- ✅ Iconografía consistente (Fluent Icons)

#### 2. **Jerarquía Visual**
```
Títulos principales (20px, Bold)
  └─ Subtítulos (16px, SemiBold)
      └─ Body text (14px, Regular)
         └─ Secondary text (12px, Regular, Gray)
```

#### 3. **Accesibilidad**
- ✅ Contraste mínimo WCAG AA (4.5:1 para texto)
- ✅ Nombres ARIA descriptivos en controles
- ✅ Navegación por teclado completa (Tab, Enter, Esc)
- ✅ Soporte para lectores de pantalla
- ✅ Tamaño mínimo de botón: 32px x 32px

#### 4. **Estados Visuales**
Cada control debe tener estados claros:
- **Default:** Normal
- **Hover:** Fondo ligero + cursor pointer
- **Active:** Color primario + borde
- **Disabled:** Gray 40% + sin interacción
- **Focus:** Borde azul 2px + focus ring

#### 5. **Espaciado y Layout**
```
Márgenes externos:     16px
Padding interno:       12px
Espaciado entre items: 8px
Altura de fila:        40px
Ancho mínimo botón:    80px
```

#### 6. **Colores Recomendados**

| Uso | Color | Hex | RGBA |
|-----|-------|-----|------|
| Primario | Azul | `#0078D4` | `0, 120, 212` |
| Secundario | Verde | `#107C10` | `16, 124, 16` |
| Advertencia | Naranja | `#FFB900` | `255, 185, 0` |
| Error | Rojo | `#E81123` | `232, 17, 35` |
| Fondo | Blanco/Gris | `#FFFFFF`/`#F5F5F5` | - |
| Texto | Negro/Gris | `#000000`/`#323232` | - |

### 📱 Componentes WinUI Recomendados

| Componente | Uso | Documentación |
|-----------|-----|---------------|
| `NavigationView` | Navegación principal | [Docs](https://learn.microsoft.com/en-us/windows/apps/winui/winui3/winui-gallery) |
| `CommandBar` | Barra de acciones | [Docs](https://learn.microsoft.com/en-us/windows/apps/design/controls/command-bar) |
| `ListView` / `DataGrid` | Listados de datos | [Docs](https://learn.microsoft.com/en-us/windows/apps/design/controls/data-grid) |
| `TextBox` | Entrada de texto | [Docs](https://learn.microsoft.com/en-us/windows/apps/design/controls/text-box) |
| `ComboBox` | Selección de opciones | [Docs](https://learn.microsoft.com/en-us/windows/apps/design/controls/combo-box) |
| `RadioButtons` | Opciones mutuamente excluyentes | [Docs](https://learn.microsoft.com/en-us/windows/apps/design/controls/radio-button) |
| `CheckBox` | Selecciones múltiples | [Docs](https://learn.microsoft.com/en-us/windows/apps/design/controls/checkbox) |
| `Button` | Acciones primarias/secundarias | [Docs](https://learn.microsoft.com/en-us/windows/apps/design/controls/buttons) |
| `InfoBar` | Mensajes de estado | [Docs](https://learn.microsoft.com/en-us/windows/apps/design/controls/infobar) |
| `ProgressRing` | Indicador de carga | [Docs](https://learn.microsoft.com/en-us/windows/apps/design/controls/progress) |
| `ContentDialog` | Diálogos modales | [Docs](https://learn.microsoft.com/en-us/windows/apps/design/controls/content-dialog) |

### 🎬 Animaciones y Transiciones

- ✅ Animaciones suaves (200-300ms)
- ✅ Transiciones de pantalla (Fade/Slide)
- ✅ Hover effects sutiles
- ✅ Loading spinners para operaciones largas
- ✅ Evitar animaciones excesivas (máximo 2 simultáneas)

---

## Patrones de Implementación

### 🔄 Patrón Async/Await

**Siempre usar async para llamadas a la API:**

```csharp
public async Task LoadQuestionnairesAsync()
{
    IsLoading = true;
    try
    {
        var response = await _apiService.GetAllQuestionnairesAsync(connectionId);
        Questionnaires = new ObservableCollection<QuestionaryDto>(response);
    }
    catch (Exception ex)
    {
        ShowError($"Error al cargar cuestionarios: {ex.Message}");
    }
    finally
    {
        IsLoading = false;
    }
}
```

### 🔒 Validación en Tiempo Real

**Validar mientras el usuario escribe:**

```csharp
private string _questionText;
public string QuestionText
{
    get => _questionText;
    set
    {
        if (SetProperty(ref _questionText, value, nameof(QuestionText)))
        {
            ValidateQuestionText();
        }
    }
}

private void ValidateQuestionText()
{
    Errors = _validator.ValidateQuestionText(QuestionText);
    IsQuestionValid = Errors.Count == 0;
}
```

### 🎯 Manejo de Errores

**Estrategia de manejo de errores:**

```csharp
public enum ErrorLevel { Info, Warning, Error, Critical }

public class ErrorHandler
{
    public static async Task<T> ExecuteAsync<T>(
        Func<Task<T>> operation,
        Action<string, ErrorLevel> onError = null)
    {
        try
        {
            return await operation();
        }
        catch (ApiException ex)
        {
            onError?.Invoke($"Error API: {ex.Message}", ErrorLevel.Error);
            throw;
        }
        catch (ValidationException ex)
        {
            onError?.Invoke($"Validación: {ex.Message}", ErrorLevel.Warning);
            throw;
        }
        catch (Exception ex)
        {
            onError?.Invoke($"Error inesperado: {ex.Message}", ErrorLevel.Critical);
            throw;
        }
    }
}
```

### 🔐 Inyección de Dependencias

**Usar contenedor DI en App.xaml.cs:**

```csharp
public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; }
    
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var services = new ServiceCollection();
        
        // Servicios
        services.AddSingleton<ApiService>();
        services.AddSingleton<QuestionaryService>();
        services.AddSingleton<ValidationService>();
        
        // ViewModels
        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<QuestionaryListViewModel>();
        services.AddTransient<QuestionaryEditorViewModel>();
        
        // Views
        services.AddSingleton<MainWindow>();
        services.AddTransient<QuestionaryListView>();
        services.AddTransient<QuestionaryEditorView>();
        
        ServiceProvider = services.BuildServiceProvider();
        
        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        mainWindow.Activate();
    }
}
```

### 📡 Comunicación entre ViewModels

**Usar Messenger Pattern:**

```csharp
public class ViewModelMessenger
{
    private static Dictionary<string, Action<object>> _subscribers = new();
    
    public static void Subscribe(string message, Action<object> action)
    {
        if (_subscribers.ContainsKey(message))
            _subscribers[message] += action;
        else
            _subscribers[message] = action;
    }
    
    public static void Send(string message, object parameter = null)
    {
        if (_subscribers.ContainsKey(message))
            _subscribers[message]?.Invoke(parameter);
    }
}

// Uso:
ViewModelMessenger.Send("QuestionaryCreated", newQuestionary);
ViewModelMessenger.Subscribe("QuestionaryCreated", (obj) => 
{
    // Recargar lista
    LoadQuestionnairesAsync();
});
```

---

## Endpoints Disponibles

### 📡 Configuración Base

```
Base URL: http://localhost:5030/api/
Auth: (sin autenticación en dev, agregar si es requerido)
Content-Type: application/json
```

### 📋 CUESTIONARIOS (`/questionary`)

#### 1. Obtener Cuestionario por ID
```http
GET /api/questionary/{connection}/{id}

Parámetros:
  connection: int (identificador de BD)
  id: Guid

Response: QuestionaryDto
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Encuesta de Satisfacción",
  "description": "string (opcional)",
  "createdBy": "usuario@email.com",
  "creationDate": "2026-01-24T10:00:00Z"
}
```

#### 2. Crear Cuestionario
```http
POST /api/questionary/{connection}/New/{name}

Parámetros:
  connection: int
  name: string (máximo 255 caracteres)

Response: QuestionaryDto (mismo como arriba)
```

#### 3. Obtener por Nombre
```http
GET /api/questionary/{connection}/name/{name}

Parámetros:
  connection: int
  name: string

Response: QuestionaryDto
```

#### 4. Obtener Todos los Cuestionarios
```http
GET /api/questionary/{connection}/all

Parámetros:
  connection: int

Response: List<QuestionaryDto>
[
  { "id": "...", "name": "Cuestionario 1", ... },
  { "id": "...", "name": "Cuestionario 2", ... }
]
```

#### 5. Obtener Cuestionario Completo (sin respuestas)
```http
GET /api/questionary/{connection}/{id}/full

Parámetros:
  connection: int
  id: Guid

Response: FullQuestionaryDto
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Encuesta de Satisfacción",
  "questions": [
    {
      "id": "...",
      "questionText": "¿Cuál es su nivel de satisfacción?",
      "questionType": { "id": "...", "dotNetType": "string" },
      "questionResponses": [...],
      "distinctAnswers": [...],
      "constraints": [...]
    }
  ]
}
```

---

### ❓ PREGUNTAS (`/question`)

#### 1. Crear Preguntas
```http
POST /api/question/new/{connection}?questionaryId={id}

Parámetros:
  connection: int (ruta)
  questionaryId: Guid (query)

Body: List<QuestionCreationDto>
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "questionText": "¿Cuál es su nombre?",
    "questionType": {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "dotNetType": "System.String"
    },
    "constraints": [
      {
        "id": "550e8400-e29b-41d4-a716-446655440000",
        "questionId": "550e8400-e29b-41d4-a716-446655440000",
        "policy": {
          "id": "550e8400-e29b-41d4-a716-446655440000",
          "name": "Validación de Email"
        },
        "policyRecords": [
          {
            "id": "550e8400-e29b-41d4-a716-446655440000",
            "constraintId": "550e8400-e29b-41d4-a716-446655440000",
            "value": "pattern:email"
          }
        ]
      }
    ]
  }
]

Response: List<QuestionDto>
```

#### 2. Obtener Preguntas del Cuestionario
```http
GET /api/question/{connection}/get?questionaryId={id}

Parámetros:
  connection: int
  questionaryId: Guid (query)

Response: List<QuestionDto>
```

#### 3. Obtener Preguntas con Respuestas Específicas
```http
POST /api/question/{connection}?questionaryId={id}

Body: List<Guid> (IDs de respuestas)

Response: List<QuestionDto>
```

---

### 📝 RESPUESTAS GLOBALES (`/answer`)

#### 1. Crear Respuesta (Sesión)
```http
POST /api/answer/{connection}?
  questionaryId={questionaryId}&
  user={usuario}&
  cardId={cardId}

Parámetros:
  connection: int (ruta)
  questionaryId: Guid (query)
  user: string (query) - email o usuario
  cardId: int (query) - opcional

Response: AnswerDto
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "questionaryId": "550e8400-e29b-41d4-a716-446655440000",
  "user": "usuario@email.com",
  "cardId": 123,
  "answerStatus": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "UNFINISHED",
    "answer_status": "UNFINISHED"
  }
}
```

#### 2. Obtener Respuesta por ID
```http
GET /api/answer/{connection}/{id}

Parámetros:
  connection: int
  id: Guid

Response: AnswerDto
```

#### 3. Cambiar Estado de Respuesta(s)
```http
PUT /api/answer/setStatus

Body: SetAnswerStatusCommand
{
  "connectionId": 1,
  "answersId": [
    "550e8400-e29b-41d4-a716-446655440000",
    "650e8400-e29b-41d4-a716-446655440001"
  ],
  "ANSWER_STATUS": "COMPLETED"
}

Response: Unit (void)

Estados válidos: COMPLETED, UNFINISHED, PENDING, CANCELLED
```

---

### 🔄 RESPUESTAS POR PREGUNTA (`/questionresponse`)

#### 1. Crear Respuestas a Preguntas
```http
POST /api/questionresponse/{connection}/response?
  questionaryId={id}&
  currentAnswerId={id}&
  newCurrentAnswerStatus={status}&
  answersId={id1}&answersId={id2}

Body: List<CreateResponseBody>
[
  {
    "questionId": "550e8400-e29b-41d4-a716-446655440000",
    "response": "Mi respuesta",
    "metadata": "{ \"time\": 5000 }" // opcional
  }
]

Response: List<QuestionResponseDto>
```

#### 2. Actualizar Respuesta a Pregunta
```http
PATCH /api/questionresponse/{connection}/response?
  questionResponseId={id}&
  response={valor}&
  metadata={json}

Parámetros:
  connection: int
  questionResponseId: Guid (query)
  response: string (query)
  metadata: string (query) - JSON serializado (opcional)

Response: QuestionResponseDto
```

#### 3. Eliminar Respuesta a Pregunta
```http
DELETE /api/questionresponse/{connection}/{id}

Parámetros:
  connection: int
  id: Guid

Response: bool (true si se eliminó)
```

---

### 🏷️ TIPOS DE PREGUNTA (`/questiontype`)

#### 1. Obtener Tipo de Pregunta
```http
GET /api/questiontype/{connection}/{id}

Response: QuestionTypeDto
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "dotNetType": "System.String"
}
```

#### 2. Crear Tipo de Pregunta
```http
POST /api/questiontype/{connection}/Add?
  typeClass={class}&
  typeName={nombre}

Parámetros:
  connection: int
  typeClass: string (ej: "System.String")
  typeName: string (ej: "Texto Corto")

Response: QuestionTypeDto
```

#### 3. Obtener Todos los Tipos
```http
GET /api/questiontype/{connection}/all

Response: List<QuestionTypeDto>
[
  { "id": "...", "dotNetType": "System.String" },
  { "id": "...", "dotNetType": "System.Boolean" },
  { "id": "...", "dotNetType": "System.Int32" }
]
```

---

### 📎 ADJUNTOS (`/attachment`)

#### 1. Crear Adjunto
```http
POST /api/attachment/{connection}/new

Body: AttachmentDto / blbEntry
{
  "storage": "azure",
  "blb_Attachment": "contenido_base64",
  "blb_container": "questionary-files",
  "filename": "documento.pdf",
  "contentType": "application/pdf",
  "confirmationFlag": true
}

Response: AttachmentDto
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "storage": "azure",
  "filename": "documento.pdf",
  "contentType": "application/pdf"
}
```

#### 2. Obtener Adjunto
```http
GET /api/attachment/{connection}/{id}

Response: AttachmentDto
```

---

### 🔐 POLÍTICAS (`/policy`)

#### 1. Obtener Todas las Políticas
```http
GET /api/policy/{connection}/all

Response: List<PolicyDto>
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Validación de Email"
  },
  {
    "id": "650e8400-e29b-41d4-a716-446655440001",
    "name": "Validación de Teléfono"
  }
]
```

---

### ℹ️ INFORMACIÓN (`/common`)

#### 1. Obtener Versión de API
```http
GET /api/common/version

Response: VersionDto
{
  "productId": "550e8400-e29b-41d4-a716-446655440000",
  "version": "1.0.0",
  "environment": "Development",
  "osPlatform": "Windows",
  "targetFramework": ".NET 6.0"
}
```

---

## Estructura de Datos (DTOs)

### QuestionaryDto
```csharp
public class QuestionaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CreatedBy { get; set; }
    public DateTimeOffset? CreationDate { get; set; }
}
```

### FullQuestionaryDto
```csharp
public class FullQuestionaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public IEnumerable<QuestionDto> Questions { get; set; }
}
```

### QuestionDto
```csharp
public class QuestionDto
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; }
    public QuestionTypeDto QuestionType { get; set; }
    public ICollection<QuestionResponseDto> QuestionResponses { get; set; }
    public ICollection<AnswerDto> DistinctAnswers { get; set; }
    public ICollection<ConstraintDto> Constraints { get; set; }
}
```

### QuestionTypeDto
```csharp
public class QuestionTypeDto
{
    public Guid Id { get; set; }
    public string DotNetType { get; set; } // System.String, System.Boolean, etc.
}
```

### AnswerDto
```csharp
public class AnswerDto
{
    public Guid Id { get; set; }
    public Guid QuestionaryId { get; set; }
    public string User { get; set; }
    public int CardId { get; set; }
    public AnswerStatusDto AnswerStatus { get; set; }
}
```

### AnswerStatusDto
```csharp
public class AnswerStatusDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ANSWER_STATUS AnswerStatus { get; set; } // COMPLETED, UNFINISHED, PENDING, CANCELLED
}
```

### QuestionResponseDto
```csharp
public class QuestionResponseDto
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public Guid AnswerId { get; set; }
    public string Response { get; set; }
    public string Metadata { get; set; } // JSON
    public ICollection<AnswerDto> Answers { get; set; }
}
```

### ConstraintDto
```csharp
public class ConstraintDto
{
    public Guid Id { get; set; }
    public Guid? QuestionId { get; set; }
    public PolicyDto Policy { get; set; }
    public IEnumerable<PolicyRecordsDto> PolicyRecords { get; set; }
}
```

### PolicyDto
```csharp
public class PolicyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
```

### PolicyRecordsDto
```csharp
public class PolicyRecordsDto
{
    public Guid Id { get; set; }
    public Guid ConstraintId { get; set; }
    public string Value { get; set; }
}
```

### AttachmentDto
```csharp
public class AttachmentDto
{
    public Guid? Id { get; set; }
    public string Storage { get; set; }
    public string Blb_Attachment { get; set; }
    public string Blb_Container { get; set; }
    public string Filename { get; set; }
    public string ContentType { get; set; }
    public bool ConfirmationFlag { get; set; }
}
```

---

## Flujos de Negocio

### 🎯 Flujo 1: Crear un Nuevo Cuestionario

```
1. Usuario selecciona "Crear Cuestionario"
   ↓
2. Se abre diálogo para ingresar nombre
   ↓
3. POST /api/questionary/{connection}/New/{name}
   ↓
4. Recibir QuestionaryDto con ID
   ↓
5. Navegar a editor de preguntas
   ↓
6. Mostrar cuestionario vacío listo para agregar preguntas
```

**Validaciones:**
- Nombre no vacío (mín. 3 caracteres)
- Nombre máximo 255 caracteres
- Nombre único (verificar en lista)

---

### 📝 Flujo 2: Agregar Preguntas al Cuestionario

```
1. Usuario en editor de cuestionario
   ↓
2. Selecciona "Agregar Pregunta"
   ↓
3. Se abre panel para crear pregunta
   ├─ Ingresar texto de pregunta
   ├─ Seleccionar tipo (Text, Choice, Boolean, etc.)
   ├─ Opcionalmente agregar restricciones
   ↓
4. POST /api/questiontype/{connection}/all (obtener tipos disponibles)
   ↓
5. POST /api/question/new/{connection}?questionaryId={id}
   con List<QuestionCreationDto>
   ↓
6. Recibir List<QuestionDto>
   ↓
7. Refrescar lista de preguntas en el editor
```

**Validaciones:**
- Texto de pregunta no vacío
- Mínimo 5 caracteres
- Máximo 500 caracteres
- Tipo de pregunta seleccionado

---

### ✅ Flujo 3: Responder a un Cuestionario (Usuario Final)

```
1. Usuario accede al cuestionario
   ↓
2. GET /api/questionary/{connection}/{id}/full
   Obtener todas las preguntas
   ↓
3. POST /api/answer/{connection}?questionaryId={id}&user={email}&cardId={cardId}
   Crear sesión de respuesta (Answer)
   ↓
4. Para cada pregunta:
   ├─ POST /api/questionresponse/{connection}/response
   │  con respuesta del usuario
   └─ Mostrar validaciones en tiempo real
   ↓
5. Al finalizar:
   ├─ PUT /api/answer/setStatus
   └─ Cambiar estado a COMPLETED
   ↓
6. Mostrar pantalla de confirmación
```

**Estados de Respuesta:**
- `UNFINISHED` - Iniciado pero no completado
- `PENDING` - Completado, pendiente de revisión
- `COMPLETED` - Completado y validado
- `CANCELLED` - Cancelado

---

### 🔍 Flujo 4: Gestionar Restricciones (Constraints)

```
1. Usuario en editor de pregunta
   ↓
2. Selecciona "Agregar Restricción"
   ↓
3. GET /api/policy/{connection}/all
   Obtener políticas disponibles
   ↓
4. Usuario selecciona política
   ↓
5. Mostrar valores permitidos (PolicyRecords)
   ↓
6. Guardar constraint con la pregunta
   en QuestionCreationDto
   ↓
7. Validar respuestas contra constraints
   al responder preguntas
```

---

## Guía de Estilo Visual

### 🎨 Temas Disponibles

#### Tema Light (Predeterminado)
```xaml
<!-- Resources/Themes/Light.xaml -->
<ResourceDictionary>
    <Color x:Key="PrimaryColor">#0078D4</Color>
    <Color x:Key="SecondaryColor">#107C10</Color>
    <Color x:Key="BackgroundColor">#FFFFFF</Color>
    <Color x:Key="TextColor">#323232</Color>
    <Color x:Key="BorderColor">#E1E1E1</Color>
    <SolidColorBrush x:Key="PrimaryBrush" Color="{StaticResource PrimaryColor}"/>
    <SolidColorBrush x:Key="BackgroundBrush" Color="{StaticResource BackgroundColor}"/>
</ResourceDictionary>
```

#### Tema Dark
```xaml
<!-- Resources/Themes/Dark.xaml -->
<ResourceDictionary>
    <Color x:Key="PrimaryColor">#60CDFF</Color>
    <Color x:Key="BackgroundColor">#1E1E1E</Color>
    <Color x:Key="TextColor">#FFFFFF</Color>
    <Color x:Key="BorderColor">#3F3F3F</Color>
</ResourceDictionary>
```

### 🧩 Componentes Estándar

#### Botón Primario
```xaml
<Button 
    Content="Crear Cuestionario"
    Background="{StaticResource PrimaryBrush}"
    Foreground="White"
    Padding="12,8"
    MinWidth="120"
    CornerRadius="4"
    Command="{Binding CreateQuestionaryCommand}"/>
```

#### Campo de Texto
```xaml
<TextBox 
    PlaceholderText="Ingrese el nombre del cuestionario"
    Text="{Binding QuestionaryName, Mode=TwoWay, UpdateTrigger=PropertyChanged}"
    Padding="12,10"
    Height="40"
    CornerRadius="4"/>
```

#### Lista de Cuestionarios
```xaml
<DataGrid 
    ItemsSource="{Binding Questionnaires}"
    SelectedItem="{Binding SelectedQuestionnaire, Mode=TwoWay}"
    RowHeight="48"
    AutoGenerateColumns="False">
    <DataGrid.Columns>
        <DataGridTextColumn Header="Nombre" Binding="{Binding Name}" Width="*"/>
        <DataGridTextColumn Header="Creado Por" Binding="{Binding CreatedBy}" Width="150"/>
        <DataGridTextColumn Header="Fecha" Binding="{Binding CreationDate, StringFormat=dd/MM/yyyy}" Width="120"/>
    </DataGrid.Columns>
</DataGrid>
```

#### Indicador de Carga
```xaml
<ProgressRing 
    IsActive="{Binding IsLoading}"
    Foreground="{StaticResource PrimaryBrush}"
    Width="40" Height="40"/>
```

#### Mensaje de Estado
```xaml
<InfoBar 
    Title="Éxito"
    Message="Cuestionario creado correctamente"
    Severity="Success"
    IsOpen="{Binding ShowSuccessMessage}"
    Foreground="Green"/>
```

---

## Técnicas Avanzadas de UX

### 1. **Drag and Drop para Organizar Preguntas**

```csharp
// ViewModel
public class QuestionaryEditorViewModel : ViewModelBase
{
    private ObservableCollection<QuestionDto> _questions;
    
    public void ReorderQuestions(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || fromIndex >= Questions.Count) return;
        if (toIndex < 0 || toIndex >= Questions.Count) return;
        
        var item = Questions[fromIndex];
        Questions.RemoveAt(fromIndex);
        Questions.Insert(toIndex, item);
        
        OnPropertyChanged(nameof(Questions));
    }
}
```

```xaml
<!-- XAML con drag-drop -->
<ListView 
    ItemsSource="{Binding Questions}"
    AllowDrop="True"
    Drop="Questions_Drop"
    DragOver="Questions_DragOver">
</ListView>
```

### 2. **Búsqueda y Filtrado en Tiempo Real**

```csharp
private string _searchText;
public string SearchText
{
    get => _searchText;
    set
    {
        if (SetProperty(ref _searchText, value, nameof(SearchText)))
        {
            FilterQuestionnaires();
        }
    }
}

private void FilterQuestionnaires()
{
    var filtered = _allQuestionnaires
        .Where(q => q.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
        .ToList();
    
    Questionnaires = new ObservableCollection<QuestionaryDto>(filtered);
}
```

### 3. **Undo/Redo en Editor**

```csharp
public class EditorHistory
{
    private Stack<EditorState> _undoStack = new();
    private Stack<EditorState> _redoStack = new();
    
    public void SaveState(EditorState state)
    {
        _undoStack.Push(state);
        _redoStack.Clear();
    }
    
    public EditorState Undo()
    {
        if (_undoStack.Count == 0) return null;
        var state = _undoStack.Pop();
        _redoStack.Push(state);
        return _undoStack.Peek();
    }
    
    public EditorState Redo()
    {
        if (_redoStack.Count == 0) return null;
        var state = _redoStack.Pop();
        _undoStack.Push(state);
        return state;
    }
}
```

### 4. **Autoguardado**

```csharp
private DispatcherTimer _autoSaveTimer;

public void InitializeAutoSave()
{
    _autoSaveTimer = new DispatcherTimer();
    _autoSaveTimer.Interval = TimeSpan.FromSeconds(30);
    _autoSaveTimer.Tick += async (s, e) => await SaveQuestionaryAsync();
    _autoSaveTimer.Start();
}

public async Task SaveQuestionaryAsync()
{
    if (IsDirty)
    {
        await _questionaryService.SaveAsync(CurrentQuestionary);
        IsDirty = false;
    }
}
```

### 5. **Validación Contextual**

```csharp
public class ContextualValidator
{
    public List<ValidationMessage> ValidateQuestion(QuestionDto question)
    {
        var messages = new List<ValidationMessage>();
        
        if (string.IsNullOrWhiteSpace(question.QuestionText))
            messages.Add(new ValidationMessage("error", "El texto es requerido"));
        
        if (question.QuestionText.Length < 5)
            messages.Add(new ValidationMessage("warning", "La pregunta es muy corta"));
        
        if (question.Constraints?.Count > 5)
            messages.Add(new ValidationMessage("info", "Considera simplificar las restricciones"));
        
        return messages;
    }
}
```

### 6. **Preview en Tiempo Real**

```xaml
<!-- Vista dual: Editor + Preview -->
<Grid ColumnDefinitions="*,*">
    <!-- Editor -->
    <StackPanel Grid.Column="0" Padding="16">
        <!-- Controles de edición -->
    </StackPanel>
    
    <!-- Preview (como vería el usuario) -->
    <ScrollViewer Grid.Column="1" Background="{StaticResource BorderBrush}">
        <StackPanel Padding="16">
            <!-- Previsualización del cuestionario -->
        </StackPanel>
    </ScrollViewer>
</Grid>
```

---

## Checklist de Implementación

### ✅ Fase 1: Configuración Base
- [ ] Crear proyecto WPF con WinUI 3
- [ ] Configurar inyección de dependencias
- [ ] Crear estructura de carpetas
- [ ] Implementar ViewModelBase
- [ ] Configurar temas Light/Dark
- [ ] Agregar Fluent Icons

### ✅ Fase 2: Servicios API
- [ ] Crear ApiService
- [ ] Implementar QuestionaryService
- [ ] Implementar QuestionService
- [ ] Implementar AnswerService
- [ ] Implementar ResponseService
- [ ] Manejo centralizado de errores
- [ ] Manejo de timeout y reintentos

### ✅ Fase 3: Pantalla de Gestión de Cuestionarios
- [ ] Vista de lista de cuestionarios
- [ ] Búsqueda y filtrado
- [ ] Crear nuevo cuestionario
- [ ] Editar cuestionario
- [ ] Eliminar cuestionario
- [ ] Exportar cuestionario
- [ ] Duplicar cuestionario

### ✅ Fase 4: Editor de Cuestionarios
- [ ] Vista principal del editor
- [ ] Agregar preguntas
- [ ] Editar preguntas
- [ ] Eliminar preguntas
- [ ] Reordenar preguntas (drag-drop)
- [ ] Previsualización
- [ ] Autoguardado

### ✅ Fase 5: Gestión de Preguntas Avanzada
- [ ] Selector de tipos de pregunta
- [ ] Editor de restricciones (constraints)
- [ ] Gestor de políticas
- [ ] Validación en tiempo real
- [ ] Sugerencias inteligentes
- [ ] Plantillas de preguntas

### ✅ Fase 6: Recopilación de Respuestas
- [ ] Formulario de respuesta
- [ ] Validación de respuestas
- [ ] Navegación entre preguntas
- [ ] Guardado progresivo
- [ ] Indicador de progreso
- [ ] Confirmación de envío

### ✅ Fase 7: Análisis de Respuestas
- [ ] Lista de respuestas recibidas
- [ ] Vista de detalle de respuesta
- [ ] Estadísticas básicas
- [ ] Gráficos de resultados
- [ ] Exportación de datos
- [ ] Filtrado por estado

### ✅ Fase 8: Pulido y Optimización
- [ ] Pruebas de usabilidad
- [ ] Optimización de rendimiento
- [ ] Accesibilidad (WCAG AA)
- [ ] Soporte multiidioma
- [ ] Documentación de usuario
- [ ] Empaquetado y distribución

---

## Recursos y Referencias

### 📚 Documentación Oficial

| Recurso | URL |
|---------|-----|
| WPF | [learn.microsoft.com/en-us/dotnet/desktop/wpf](https://learn.microsoft.com/en-us/dotnet/desktop/wpf) |
| MVVM Toolkit | [github.com/CommunityToolkit/dotnet](https://github.com/CommunityToolkit/dotnet) |
| Fluent Design | [microsoft.com/design/fluent](https://www.microsoft.com/design/fluent) |
| Fluent Icons | [fluenticons.com](https://fluenticons.com) |

### 🛠️ Herramientas Recomendadas

```
Visual Studio 2022 Community (o superior)
├─ Extensiones:
│  ├─ XAML Styler
│  ├─ Productivity Power Tools
│  ├─ ReSharper (opcional)
│  └─ NuGet Package Manager
├─ NuGet Packages:
│  ├─ Microsoft.UI (WinUI 3)
│  ├─ CommunityToolkit.Mvvm
│  ├─ CommunityToolkit.WinUI.Controls
│  ├─ Newtonsoft.Json
│  ├─ AutoMapper
│  ├─ Serilog
│  └─ RestSharp / HttpClientFactory
└─ Herramientas:
   ├─ Postman (para probar API)
   ├─ Fiddler (para debugging)
   └─ XamlSpy (para inspeccionar XAML)
```

### 📦 NuGet Packages Esenciales

```xml
<!-- .csproj -->
<ItemGroup>
    <PackageReference Include="Microsoft.UI" Version="3.0.0" />
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
    <PackageReference Include="CommunityToolkit.WinUI.Controls" Version="8.0.24031" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
    <PackageReference Include="AutoMapper" Version="13.0.0" />
    <PackageReference Include="Serilog" Version="3.1.1" />
    <PackageReference Include="RestSharp" Version="107.3.0" />
</ItemGroup>
```

### 🎓 Temas de Aprendizaje

1. **MVVM Pattern Deep Dive**
   - [microsoft.com/en-us/developers](https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern)

2. **Async/Await Best Practices**
   - [Stephen Cleary's Blog](https://blog.stephencleary.com/)

3. **WPF Performance**
   - [docs.microsoft.com/wpf-performance](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/advanced/optimizing-wpf-application-performance)

4. **XAML Best Practices**
   - [XAML in WPF - Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/xaml/)

---

## Convenciones de Código

### 🎯 Nomenclatura

```csharp
// Clases: PascalCase
public class QuestionaryEditorViewModel { }

// Propiedades públicas: PascalCase
public ObservableCollection<QuestionDto> Questions { get; set; }

// Campos privados: _camelCase
private string _searchText;

// Constantes: UPPER_CASE
private const int MAX_QUESTION_LENGTH = 500;

// Propiedades booleanas: Is/Has/Can
public bool IsLoading { get; set; }
public bool HasErrors { get; set; }
public bool CanSave { get; set; }
```

### 📝 Documentación

```csharp
/// <summary>
/// Crea un nuevo cuestionario con el nombre especificado.
/// </summary>
/// <param name="connectionId">Identificador de la base de datos</param>
/// <param name="name">Nombre del cuestionario (3-255 caracteres)</param>
/// <returns>QuestionaryDto con los datos del cuestionario creado</returns>
/// <exception cref="ArgumentException">Si el nombre está vacío o es muy largo</exception>
/// <exception cref="ApiException">Si hay error en la comunicación con la API</exception>
public async Task<QuestionaryDto> CreateQuestionaryAsync(int connectionId, string name)
{
    // Implementación
}
```

### 🔍 Logging

```csharp
private readonly ILogger<QuestionaryEditorViewModel> _logger;

public async Task LoadQuestionaryAsync(Guid id)
{
    _logger.LogInformation("Iniciando carga de cuestionario: {QuestionaryId}", id);
    
    try
    {
        var questionary = await _questionaryService.GetFullQuestionaryAsync(id);
        _logger.LogInformation("Cuestionario cargado exitosamente");
        CurrentQuestionary = questionary;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error al cargar cuestionario: {QuestionaryId}", id);
        ShowError("No se pudo cargar el cuestionario");
    }
}
```

---

## Conclusión

Esta documentación proporciona una guía integral para desarrollar un frontend moderno, intuitivo y profesional para la gestión de cuestionarios usando WPF y WinUI. 

**Puntos clave a recordar:**
- ✅ Siempre incluir el parámetro `connection` en las llamadas API
- ✅ Usar MVVM y mantener la lógica de negocio fuera de las vistas
- ✅ Validar entrada del usuario en tiempo real
- ✅ Proporcionar feedback visual claro en todas las operaciones
- ✅ Mantener la consistencia visual con los temas
- ✅ Manejar errores de forma elegante y usuario-friendly
- ✅ Usar async/await para no bloquear la UI
- ✅ Seguir las pautas de accesibilidad (WCAG)

**¡Éxito en el desarrollo!** 🚀

---

## 🚀 Prompt para GitHub Copilot - Jumpstart Project

```
Create a modern WPF application with WinUI 3 for managing questionnaires (surveys/forms) similar to Google Forms but for enterprise use. 

The application should connect to an existing REST API at http://localhost:5030/api/ that implements a Clean Architecture backend with CQRS pattern. All API calls require a "connection" parameter (int) to identify the database tenant.

Architecture Requirements:
- Use MVVM pattern with CommunityToolkit.Mvvm
- Implement dependency injection with Microsoft.Extensions.DependencyInjection
- Create separate layers: Views (XAML), ViewModels, Services, Models, Converters
- Use async/await for all API operations
- Implement proper error handling with user-friendly messages

Core Features to Implement:
1. Questionnaire Management - List, create, edit, and delete questionnaires with search/filter capabilities
2. Question Editor - Visual editor to add/edit questions with drag-and-drop reordering, support for multiple question types (text, boolean, numeric, date, file upload), and constraint/validation rules
3. Response Collection - Form interface for end-users to answer questionnaires with real-time validation, progress tracking, auto-save functionality, and state management (UNFINISHED, PENDING, COMPLETED, CANCELLED)
4. Response Analysis - View submitted responses with filtering by status and basic statistics

Key Data Models:
- Questionary: id (Guid), name (string), questions (collection)
- Question: id (Guid), questionText (string), questionType (with dotNetType), constraints (validation rules)
- Answer: id (Guid), questionaryId (Guid), user (string), answerStatus (enum)
- QuestionResponse: id (Guid), questionId (Guid), answerId (Guid), response (string), metadata (JSON)

UI/UX Requirements:
- Follow Fluent Design System principles
- Implement Light/Dark themes with smooth transitions
- Use NavigationView for main navigation
- Colors: Primary #0078D4 (blue), Secondary #107C10 (green), Error #E81123 (red)
- Minimum touch target: 32x32px, consistent spacing (8px/12px/16px)
- Accessibility: WCAG AA compliance, keyboard navigation, screen reader support
- Animations: 200-300ms transitions, subtle hover effects, loading indicators

Technical Stack:
- Framework: .NET 6+ WPF with WinUI 3 (Microsoft.UI package)
- MVVM: CommunityToolkit.Mvvm for ViewModelBase, RelayCommand, ObservableProperty
- HTTP Client: RestSharp or HttpClientFactory for API calls
- JSON: Newtonsoft.Json for serialization
- Logging: Serilog with file and debug output
- UI Controls: CommunityToolkit.WinUI.Controls for enhanced controls

Project Structure:
SurveyManagementApp/
├── Views/ (MainWindow, QuestionaryListView, QuestionaryEditorView, ResponseManagerView)
├── ViewModels/ (corresponding ViewModels with proper data binding)
├── Services/ (ApiService, QuestionaryService, QuestionService, ResponseService, ValidationService)
├── Models/ (DTOs matching API contracts)
├── Converters/ (value converters for XAML binding)
├── Resources/ (Themes, Strings for i18n, Icons)
└── Helpers/ (utility classes)

Critical Implementation Notes:
- The "connection" parameter must be included in almost all API calls
- Answer represents a user session (one attempt), QuestionResponse represents individual question answers within that session
- Constraints contain Policy and PolicyRecords for validation rules (email patterns, numeric ranges, allowed values)
- Use FullQuestionaryDto for editing (includes all questions), QuestionaryDto for listing (lightweight)
- Support incremental save: users can save partial responses and continue later
- Implement proper state management: questions can be added to questionnaires that already have responses

Start with:
1. Basic project structure with DI container
2. ApiService with connection parameter support
3. MainWindow with NavigationView
4. QuestionaryListView showing all questionnaires from GET /api/questionary/{connection}/all
5. Simple create questionnaire dialog posting to POST /api/questionary/{connection}/New/{name}

Include comprehensive XML documentation, follow C# naming conventions (PascalCase for public, _camelCase for private fields), and add inline comments for complex logic. Implement proper async error handling with try-catch blocks and user notifications via InfoBar controls.
```

---

## 🤖 Prompt para GitHub Copilot CLI - Desarrollo Avanzado

```bash
# Para usar con: gh copilot suggest -t shell
# o dentro de VS Code Copilot Chat con @ workspace context
```

### Prompt Completo para Copilot Agent

```
You are an expert WPF + WinUI 3 developer tasked with building a production-ready questionnaire management system.

CRITICAL: You MUST read and strictly follow the complete technical documentation located at:
\FRONTEND_TECHNICAL_DOCUMENTATION.md

This file contains ALL specifications including API endpoints, data models, UI/UX rules, validation patterns, and architectural decisions. Refer to it constantly.

ADVANCED REQUIREMENTS - Use Latest Techniques:

1. GRAPH-BASED STATE MANAGEMENT
   - Implement a state machine using a directed graph for Answer status transitions
   - States: UNFINISHED → PENDING → COMPLETED (with CANCELLED as exit state)
   - Use Stateless library (NuGet: Stateless) to manage state transitions
   - Track state history with timestamps for auditing
   - Validate transitions before executing (prevent invalid state changes)
   - Example:
     ```csharp
     var answerStateMachine = new StateMachine<AnswerState, AnswerTrigger>(AnswerState.Unfinished);
     answerStateMachine.Configure(AnswerState.Unfinished)
         .Permit(AnswerTrigger.Complete, AnswerState.Pending)
         .Permit(AnswerTrigger.Cancel, AnswerState.Cancelled);
     answerStateMachine.Configure(AnswerState.Pending)
         .Permit(AnswerTrigger.Approve, AnswerState.Completed)
         .Permit(AnswerTrigger.Reject, AnswerState.Unfinished);
     ```

2. SESSION MANAGEMENT WITH CHECKPOINTS
   - Implement Mark Checkpointing pattern for auto-save and recovery
   - Create SessionManager that saves state snapshots every 30 seconds or on specific triggers
   - Store session data locally using System.Text.Json with encryption (System.Security.Cryptography)
   - Session structure:
     ```csharp
     public class SessionCheckpoint
     {
         public Guid CheckpointId { get; set; }
         public DateTime Timestamp { get; set; }
         public Guid AnswerId { get; set; }
         public Dictionary<Guid, string> QuestionResponses { get; set; }
         public int ProgressPercentage { get; set; }
         public string UserAgent { get; set; }
         public byte[] Hash { get; set; } // For integrity verification
     }
     ```
   - Implement recovery dialog on app restart if unfinished sessions exist
   - Allow user to "Continue where I left off" or "Start fresh"

3. HISTORY TRACKING WITH TEMPORAL GRAPH
   - Use temporal graph pattern to track ALL changes over time
   - Every edit to a questionnaire creates a new version node
   - Link versions with timestamps and user information
   - Enable "View history" feature showing timeline of changes
   - Implement diff viewer to compare versions side-by-side
   - Structure:
     ```csharp
     public class QuestionaryVersion
     {
         public Guid VersionId { get; set; }
         public Guid QuestionaryId { get; set; }
         public int VersionNumber { get; set; }
         public DateTime CreatedAt { get; set; }
         public string CreatedBy { get; set; }
         public string ChangeDescription { get; set; }
         public JObject Snapshot { get; set; } // Complete state
         public Guid? PreviousVersionId { get; set; }
         public List<ChangeOperation> Changes { get; set; }
     }
     ```

4. INTELLIGENT CACHING WITH GRAPH INVALIDATION
   - Implement cache dependency graph using QuickGraph library
   - When Questionary changes, invalidate all dependent cached items (Questions, Constraints, etc.)
   - Use IMemoryCache with custom eviction policies based on relationships
   - Cache structure:
     ```csharp
     public class GraphCache<TKey, TValue>
     {
         private readonly IMemoryCache _cache;
         private readonly BidirectionalGraph<CacheNode, CacheEdge> _dependencyGraph;
         
         public void InvalidateNode(TKey key)
         {
             // Find all descendants in graph and evict them
             var descendants = GetAllDescendants(key);
             foreach (var descendant in descendants)
                 _cache.Remove(descendant);
         }
     }
     ```

5. REAL-TIME VALIDATION WITH REACTIVE EXTENSIONS
   - Use System.Reactive (Rx.NET) for reactive validation
   - Debounce user input (500ms) before triggering validation
   - Throttle API calls to prevent rate limiting
   - Combine multiple validation streams
   - Example:
     ```csharp
     Observable
         .FromEventPattern<TextChangedEventArgs>(questionTextBox, nameof(TextBox.TextChanged))
         .Throttle(TimeSpan.FromMilliseconds(500))
         .Select(e => questionTextBox.Text)
         .DistinctUntilChanged()
         .ObserveOn(SynchronizationContext.Current)
         .Subscribe(async text => await ValidateQuestionTextAsync(text));
     ```

6. UNDO/REDO WITH COMMAND PATTERN GRAPH
   - Implement Command pattern with graph-based history
   - Each command forms a node in the undo graph
   - Support branching: if user undoes then makes new change, create branch
   - Visualize undo history as tree structure
   - Commands to implement:
     ```csharp
     public interface ICommand
     {
         Guid CommandId { get; }
         Guid? ParentCommandId { get; }
         DateTime ExecutedAt { get; }
         Task ExecuteAsync();
         Task UndoAsync();
         Task RedoAsync();
         string Description { get; }
     }
     
     // Concrete commands: AddQuestionCommand, DeleteQuestionCommand,
     // ModifyQuestionCommand, ReorderQuestionsCommand, etc.
     ```

7. PROGRESSIVE WEB ASSEMBLY (OPTIONAL ENHANCEMENT)
   - Consider creating a Blazor WebAssembly twin that shares the same business logic
   - Use shared .NET Standard libraries for Models, Services, Validation
   - This allows web access while maintaining WPF for desktop power users
   - Share 80% of codebase between WPF and Blazor

8. TELEMETRY AND ANALYTICS GRAPH
   - Track user interactions as event graph
   - Events: QuestionaryOpened, QuestionAdded, ConstraintApplied, ResponseStarted, ResponseCompleted
   - Build adjacency list to analyze common user flows
   - Use this data to optimize UI/UX (show most-used features prominently)
   - Example:
     ```csharp
     public class TelemetryGraph
     {
         private readonly DirectedGraph<UserAction, ActionTransition> _flowGraph;
         
         public void RecordAction(UserAction action)
         {
             if (_lastAction != null)
                 _flowGraph.AddEdge(new ActionTransition(_lastAction, action));
             _lastAction = action;
         }
         
         public List<ActionPath> GetMostCommonPaths(int topN = 10)
         {
             // Analyze graph to find most frequent paths
         }
     }
     ```

9. CONFLICT RESOLUTION FOR CONCURRENT EDITS
   - Implement Operational Transformation (OT) or CRDT-like conflict resolution
   - If two users edit the same questionary simultaneously, merge changes intelligently
   - Use vector clocks or Lamport timestamps to order events
   - Show conflict resolution UI when automatic merge isn't possible
   - Example:
     ```csharp
     public class ConflictResolver
     {
         public QuestionaryDto Merge(QuestionaryDto local, QuestionaryDto remote, QuestionaryDto commonAncestor)
         {
             var merged = commonAncestor.Clone();
             
             // Three-way merge algorithm
             // 1. Apply local changes that don't conflict
             // 2. Apply remote changes that don't conflict
             // 3. For conflicts, present to user or use heuristics (last-write-wins, etc.)
             
             return merged;
         }
     }
     ```

10. PERFORMANCE OPTIMIZATIONS
    - Virtualize long lists (ListView.ItemsPanel with VirtualizingStackPanel)
    - Lazy load questions (only load visible + 20 buffer)
    - Use DataTemplate pooling for repeated items
    - Implement pagination for large response sets (100 per page)
    - Background thread for serialization/deserialization
    - Use Span<T> and Memory<T> for high-performance string operations

ARCHITECTURAL PATTERNS TO IMPLEMENT:

✅ Repository Pattern with Unit of Work
✅ CQRS on client side (separate read/write models if complex)
✅ Event Sourcing for audit trail (store all events, rebuild state)
✅ Mediator Pattern for loose coupling (use MediatR on client too if complex)
✅ Specification Pattern for complex validation rules
✅ Builder Pattern for constructing complex DTOs
✅ Factory Pattern for creating UI controls based on QuestionType
✅ Strategy Pattern for different validation strategies
✅ Observer Pattern for real-time updates (SignalR integration future)
✅ Memento Pattern for undo/redo state capture

TESTING REQUIREMENTS:

- Unit tests for all ViewModels (xUnit + Moq)
- Integration tests for API services (WireMock for mocking API)
- UI automation tests (Appium or WinAppDriver)
- Performance tests (BenchmarkDotNet for critical paths)
- Test coverage minimum: 80% for business logic
- Snapshot testing for complex DTOs

CODE QUALITY STANDARDS:

- Enable nullable reference types in csproj (<Nullable>enable</Nullable>)
- Use C# 10+ features: record types, global usings, file-scoped namespaces
- Async all the way (no .Result or .Wait())
- Use ValueTask<T> for hot paths
- Implement IDisposable/IAsyncDisposable properly
- Use ConfigureAwait(false) in library code
- Follow SOLID principles strictly
- Cyclomatic complexity max: 10 per method
- Lines per method max: 50
- Use Code Analyzers: StyleCop, Roslynator, SonarLint

SECURITY REQUIREMENTS:

- Never store connection parameter in plain text (use Windows Credential Manager)
- Encrypt session checkpoints with DPAPI (Data Protection API)
- Sanitize all user input before API calls (prevent injection)
- Validate all API responses (don't trust backend completely)
- Implement rate limiting on client side (max 10 requests/second)
- Use HTTPS only for API calls
- Implement certificate pinning if possible

ACCESSIBILITY (WCAG 2.1 AAA):

- All interactive elements have AutomationProperties.Name
- Keyboard navigation fully functional (Tab, Shift+Tab, Arrow keys)
- Focus indicators clearly visible (2px blue border)
- Color contrast ratio minimum 7:1 for text
- Support screen readers (Narrator, JAWS, NVDA)
- All images have alt text
- Forms have proper labels and error associations
- Support high contrast mode
- Font sizes scalable (respect system DPI settings)

DOCUMENTATION TO GENERATE:

- XML documentation for all public APIs (100% coverage)
- README.md with setup instructions
- ARCHITECTURE.md explaining design decisions
- API_INTEGRATION.md documenting endpoint usage
- USER_GUIDE.md with screenshots
- DEPLOYMENT.md with packaging instructions
- CHANGELOG.md following Keep a Changelog format

DELIVERABLES - PHASE BY PHASE:

Phase 1: Foundation (Week 1)
- [ ] Project structure with all folders
- [ ] DI container setup in App.xaml.cs
- [ ] ApiService base implementation with HttpClientFactory
- [ ] MainWindow with NavigationView skeleton
- [ ] Light/Dark theme switching
- [ ] Logging infrastructure with Serilog

Phase 2: Questionary Management (Week 2)
- [ ] QuestionaryListView with DataGrid
- [ ] Search and filter functionality
- [ ] Create/Edit/Delete questionary dialogs
- [ ] GraphCache implementation for questionaries
- [ ] SessionManager with checkpointing
- [ ] Unit tests for QuestionaryService

Phase 3: Question Editor (Week 3-4)
- [ ] Question list with drag-and-drop reordering
- [ ] Add/Edit/Delete question UI
- [ ] QuestionType selector with Factory pattern
- [ ] Constraint editor with Policy selection
- [ ] Real-time validation with Rx.NET
- [ ] Undo/Redo with Command pattern graph
- [ ] Live preview pane
- [ ] Version history viewer

Phase 4: Response Collection (Week 5)
- [ ] Form renderer based on QuestionType
- [ ] State machine for Answer status
- [ ] Progress tracking UI
- [ ] Auto-save with SessionCheckpoint
- [ ] Recovery dialog for unfinished sessions
- [ ] Metadata collection (time spent, device info)
- [ ] Submission confirmation

Phase 5: Response Analysis (Week 6)
- [ ] Response list with filtering
- [ ] Response detail view
- [ ] Basic statistics (charts with LiveCharts2)
- [ ] Export to CSV/Excel (EPPlus library)
- [ ] Conflict resolution UI

Phase 6: Polish & Optimization (Week 7-8)
- [ ] Performance optimization (virtualization, lazy loading)
- [ ] Accessibility audit and fixes
- [ ] UI/UX refinement
- [ ] Telemetry implementation
- [ ] Comprehensive testing
- [ ] Documentation completion
- [ ] Deployment packaging (MSIX)

REMEMBER:
- Constantly refer to FRONTEND_TECHNICAL_DOCUMENTATION.md for ALL specifications
- Every API call MUST include the connection parameter
- Follow the exact endpoint signatures documented
- Implement Answer (session) vs QuestionResponse (individual answer) correctly
- Handle all four Answer states: UNFINISHED, PENDING, COMPLETED, CANCELLED
- Use Constraints with Policies for validation rules
- Maintain session checkpoints for recovery
- Track history with temporal graph pattern
- Implement state machine for Answer transitions
- Write clean, maintainable, testable code

Begin by creating the foundational project structure and confirming you understand the requirements before proceeding with implementation.
```

---

## 📝 Additional Copilot Chat Prompts for Specific Features

### For State Machine Implementation
```
@workspace Create a robust state machine for Answer status management using the Stateless library.

Requirements:
- States: UNFINISHED, PENDING, COMPLETED, CANCELLED
- Triggers: Start, Complete, Approve, Reject, Cancel
- Valid transitions (refer to FRONTEND_TECHNICAL_DOCUMENTATION.md section on Answer states)
- Log all transitions with timestamp and user
- Prevent invalid transitions with meaningful error messages
- Integrate with PUT /api/answer/setStatus endpoint
- Unit tests for all valid and invalid transitions

Include complete implementation with:
1. AnswerStateMachine class
2. Integration in AnswerService
3. UI feedback for state changes (InfoBar notifications)
4. State history tracking in database/local storage
```

### For Session Checkpoint System
```
@workspace Implement a session checkpoint system for auto-saving user progress when filling out questionnaires.

Requirements from documentation:
- Save checkpoint every 30 seconds automatically
- Save on manual trigger (Save Draft button)
- Encrypt checkpoints using Windows DPAPI
- Store locally in %APPDATA%\SurveyApp\Sessions\
- Include: AnswerId, QuestionResponses (Dictionary<Guid, string>), progress %, timestamp, hash for integrity
- On app restart, check for unfinished sessions and offer recovery
- Recovery dialog with options: Continue, Start Fresh, Discard
- Clear old checkpoints (>7 days) on app start

Create:
1. SessionCheckpoint model
2. SessionManager service
3. Recovery dialog UI
4. Background timer for auto-save
5. Unit tests for save/load/recovery scenarios
```

### For History & Versioning
```
@workspace Implement temporal graph-based version history for questionnaires.

Per FRONTEND_TECHNICAL_DOCUMENTATION.md specifications:
- Track every change to Questionary and Questions
- Create QuestionaryVersion nodes linked as temporal graph
- Store complete snapshot (JSON) + delta changes
- Enable viewing history timeline
- Implement diff viewer showing side-by-side comparison
- Allow restore to previous version (creates new version, doesn't overwrite)
- Use colors: green for additions, red for deletions, yellow for modifications

Implement:
1. QuestionaryVersion model with graph structure
2. VersioningService with graph operations
3. HistoryViewer UI with timeline
4. DiffViewer UI with side-by-side comparison
5. Restore functionality
6. Storage (local SQLite for caching, API for persistence)
```

### For Reactive Validation
```
@workspace Set up reactive validation using Rx.NET for question input.

According to documentation:
- Debounce text input 500ms before validating
- Combine multiple validation streams (local + remote)
- Show inline validation messages with severity (error, warning, info)
- Validate against Constraints and Policies
- Throttle API validation calls to max 1 per second
- Display validation spinner during async checks
- Use color coding: red border for errors, yellow for warnings

Create:
1. ReactiveValidationService using System.Reactive
2. ValidationResult model with message, severity, field
3. UI integration with TextBox (binding to HasErrors, ErrorMessage)
4. Constraint interpreters for different PolicyRecord types
5. Local validators (pattern, length, range)
6. Remote validators (uniqueness checks via API)
```

### For Conflict Resolution
```
@workspace Build a three-way merge conflict resolver for concurrent questionary edits.

As specified in documentation:
- Detect conflicts when saving (compare timestamps, version numbers)
- Use three-way merge: local changes + remote changes + common ancestor
- Auto-merge non-conflicting changes
- Present conflicts to user in clear UI
- Show: Your Change | Current Version | Merged Result
- Allow manual resolution: Accept Yours, Accept Theirs, Edit Manually
- Use vector clocks or version numbers for ordering

Implement:
1. ConflictResolver with three-way merge algorithm
2. ConflictDetectionService
3. ConflictResolutionDialog UI
4. Diff visualization component
5. Merge strategy: last-write-wins with user override
6. Tests for various conflict scenarios
```

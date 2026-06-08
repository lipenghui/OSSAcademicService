# OSSAcademicService

Academic Service is the core service for educational management in the Omni School System, handling student records, course management, scheduling, course selection, grade management, exam administration, and classroom resource scheduling.

## Overview

Academic Service is the **educational management core service** of the campus management system, responsible for student record management, course management, scheduling, course selection management, grade management, exam management, classroom resource scheduling, and other educational link responsibilities. This service is the hub of teaching operations and has close data interaction with Student Service, Faculty Service, Finance Service, etc.

## Key Features

### Core Responsibilities
- ✓ Student Record Management (student profiles, student status changes, graduation management)
- ✓ Course Management (course database, syllabus, prerequisite/co-requisite/substitute relationships)
- ✓ Scheduling System (automatic scheduling, manual adjustments, schedule changes, schedule generation)
- ✓ Course Selection Management (selection rounds, lottery mechanisms, capacity control, conflict detection)
- ✓ Grade Management (grade entry, GPA calculation, grade analysis, academic warning)
- ✓ Exam Management (exam scheduling, invigilation assignment, make-up/deferred exams)
- ✓ Classroom Resources (resource calendar, borrowing approval, utilization statistics)
- ✓ Teaching Plans and Training Programs (program version management, teaching execution plans)

### Boundaries
- ✗ Does not store user authentication information (managed by Identity Service)
- ✗ Does not handle admission processes (managed by Student Service)
- ✗ Does not manage faculty records and workload (managed by Faculty Service)
- ✗ Does not handle tuition billing and payments (managed by Finance Service)
- ✗ Does not handle access control/leave management (managed by Security Service)

## Architecture

### Domain Model Overview

```
Academic Service Domain

├── Student Record Subdomain
│   ├── StudentProfile (aggregate root)
│   ├── StatusChange (aggregate root)
│   └── GraduationAudit (aggregate root)
│
├── Course Management Subdomain
│   ├── Course (aggregate root)
│   └── TrainingPlan (aggregate root)
│
├── Scheduling Subdomain
│   ├── TeachingTask (aggregate root)
│   ├── ScheduleItem (entity)
│   └── ScheduleAdjustment (aggregate root)
│
├── Course Selection Subdomain
│   ├── SelectionRound (aggregate root)
│   └── SelectionRecord (entity)
│
├── Grades Subdomain
│   ├── ScoreRecord (aggregate root)
│   ├── ScoreRule (value object)
│   └── GpaSummary (entity)
│
├── Exam Subdomain
│   ├── ExamArrangement (aggregate root)
│   └── DeferredExam (aggregate root)
│
└── Classroom Resource Subdomain
    ├── Building (aggregate root)
    ├── Classroom (aggregate root)
    └── ClassroomBooking (aggregate root)
```

### Service Dependencies

```
                    ┌──────────────┐
                    │  Identity    │  ← JWT Authentication/Permission Validation
                    └──────┬───────┘
                           │
              ┌────────────┼────────────┐
              ▼            ▼            ▼
        ┌──────────┐ ┌──────────┐ ┌──────────┐
        │ Student  │ │ Faculty  │ │ Finance  │
        │ Service  │ │ Service  │ │ Service  │
        └──────────┘ └──────────┘ └──────────┘
```

### Database Schema

The service uses MySQL 8.0+ with a database named `db_academic`.

Key tables include:
- `t_student_profile` - Student profile table
- `t_course` - Course table
- `t_teaching_task` - Teaching task table
- `t_schedule_item` - Scheduling item table
- `t_score_record` - Score record table
- `t_exam_arrangement` - Exam arrangement table
- `t_classroom` - Classroom table

## Technical Details

- **Ports**: HTTP 5002 / gRPC 50052
- **Database**: `db_academic` (MySQL 8.0+)
- **Architecture**: Microservice
- **Domain-Driven Design**: Implements bounded contexts for different academic management areas
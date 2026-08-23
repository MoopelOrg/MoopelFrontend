# Moopel Frontend Product Requirements Document

## 1. Product Overview

**Product:** Moopel  
**Document Focus:** Frontend application  
**Frontend Technology:** Blazor  
**Backend:** ASP.NET Core Web API  
**Database:** SQL Server

Moopel is a modular personal-management application intended to bring multiple areas of a user's life into one cohesive system. The frontend is the primary user-facing experience and should make the different capabilities of Moopel feel like parts of one application rather than a collection of unrelated CRUD screens.

The frontend should provide a clear dashboard, consistent navigation, reusable UI patterns, responsive layouts, authentication/session handling, and dedicated interfaces for each major Moopel capability.

The frontend communicates with the existing Moopel API rather than implementing business logic or database access itself.

---

# 2. Product Vision

The Moopel frontend should feel like a **personal operating system**.

Instead of requiring the user to remember which tool, page, or application contains a piece of information, Moopel should provide a single place from which the user can access:

- Work and tasks
- Groups and organization
- Notes
- Tags
- Banking and transactions
- Household inventory
- Storage
- Fridges and fridge items
- Reports
- User settings
- Themes
- Other future Moopel applications

The frontend should emphasize **quick access, discoverability, consistency, and low cognitive overhead**.

---

# 3. Goals

## Primary Goals

### 3.1 Create a Unified Moopel Experience

Every module should feel like part of the same product.

Users should encounter consistent:

- Navigation
- Buttons
- Forms
- Tables
- Cards
- Dialogs
- Loading states
- Error states
- Empty states
- Typography
- Spacing
- Colors
- Icons
- Responsive behavior

### 3.2 Provide a Useful Home Dashboard

The home page should act as the user's starting point.

It should provide access to Moopel's major applications through a visual app/tile interface.

The existing frontend concept includes application tiles with properties such as:

- Label
- Availability
- Badge
- Link

The dashboard should make it immediately obvious:

1. What applications are available
2. Which applications are unavailable or unfinished
3. Which applications need attention
4. Where the user can navigate next

### 3.3 Make Navigation Predictable

The user should always understand:

- Where they are
- How they got there
- How to return
- What other related pages exist

Navigation should work consistently across all modules.

### 3.4 Support Future Modules

Moopel is intended to grow.

The frontend architecture should therefore make adding a new application/module inexpensive.

Adding a new module should primarily involve:

- Registering the module
- Adding its navigation
- Creating its pages
- Reusing existing components

It should not require redesigning the application shell.

---

# 4. Non-Goals

The frontend should **not**:

- Directly access SQL Server
- Duplicate backend business logic
- Implement authorization rules independently of the API
- Hard-code database behavior into components
- Require a completely separate UI pattern for every module
- Make every API endpoint into a separate visible page
- Optimize for an unnecessarily complex enterprise workflow

The frontend is responsible for presentation, interaction, navigation, client-side state, and communicating with the API.

---

# 5. Target User Experience

The ideal interaction model is:

```text
Open Moopel
    ↓
Dashboard
    ↓
Choose an application
    ↓
Application landing page
    ↓
View / create / edit information
    ↓
Return to application or dashboard
```

The user should rarely need to understand the underlying API structure.

For example, the user should think:

> "I want to see my bank accounts."

rather than:

> "I need to navigate to the BankAccountsController endpoint."

---

# 6. Application Shell

The application shell is the persistent structure surrounding the individual pages.

It should provide:

- Global navigation
- Application navigation
- Main content area
- User/session controls
- Responsive behavior
- Global notifications/messages
- Consistent page structure

Conceptually:

```text
┌──────────────────────────────────────────────────────────┐
│ Moopel                                      User / Menu  │
├──────────────────────────────────────────────────────────┤
│                                                          │
│ Global/App Navigation                                    │
│                                                          │
├──────────────────────────────────────────────────────────┤
│                                                          │
│                       Page Content                        │
│                                                          │
│                                                          │
└──────────────────────────────────────────────────────────┘
```

The shell should remain visually stable while the content changes.

---

# 7. Dashboard

The dashboard is the primary entry point after authentication.

## Requirements

The dashboard should provide a collection of **application tiles**.

Each tile may contain:

- Application name
- Short description
- Icon
- Availability state
- Badge
- Navigation target

Example conceptual applications:

```text
┌──────────────┐ ┌──────────────┐ ┌──────────────┐
│ Work Items   │ │ Banking      │ │ Notes        │
│              │ │              │ │              │
│ Manage work  │ │ Accounts &   │ │ Personal     │
│ and tasks    │ │ transactions │ │ notes        │
└──────────────┘ └──────────────┘ └──────────────┘

┌──────────────┐ ┌──────────────┐ ┌──────────────┐
│ Fridge       │ │ Storage      │ │ Reports      │
│              │ │              │ │              │
│ Household    │ │ Stored items │ │ Information  │
│ inventory    │ │              │ │ & analytics  │
└──────────────┘ └──────────────┘ └──────────────┘
```

## Availability

Applications that are not yet available should still be representable in the dashboard.

Unavailable applications should:

- Remain visually distinct
- Not navigate to broken pages
- Clearly communicate that they are unavailable
- Avoid appearing to be broken

This supports incremental development of Moopel.

## Badges

Tiles may display badges when useful.

Examples:

- Count of pending items
- Notifications
- New items
- Attention required

Badges should only be displayed when there is meaningful information to communicate.

---

# 8. Navigation

Moopel should have two conceptual levels of navigation.

## Global Navigation

Global navigation provides access to major areas of Moopel.

Examples:

- Home
- Applications
- Settings
- User/session controls

## Application Navigation

Once inside an application, navigation should expose pages belonging to that application.

For example:

```text
Banking
├── Overview
├── Bank Accounts
├── Transactions
└── Reports
```

or:

```text
Work
├── Overview
├── Work Items
├── Groups
└── Tags
```

## Active Navigation State

The active navigation item should be determined from the current route rather than manually maintained state.

The current frontend already uses `NavigationManager` and `NavLink`; this should remain the foundation for route-aware navigation.

Navigation must correctly handle:

- Exact matches
- Parent routes
- Nested routes
- Query strings where appropriate
- Browser back/forward navigation

---

# 9. Page Structure

Pages should follow a consistent structure.

Recommended pattern:

```text
Page
├── Page header
│   ├── Title
│   ├── Description
│   └── Primary action
│
├── Filters / controls
│
├── Main content
│
└── Secondary actions / pagination
```

For example:

```text
Bank Accounts

Manage your connected bank accounts.

[ + Add Account ]

────────────────────────────────────

Accounts
┌────────────────────────────────────┐
│ Checking                           │
│ $2,450.00                          │
└────────────────────────────────────┘

┌────────────────────────────────────┐
│ Savings                            │
│ $8,200.00                          │
└────────────────────────────────────┘
```

Pages should avoid unnecessarily dense interfaces.

---

# 10. Reusable Component System

The frontend should establish reusable components before every module develops its own UI.

Core components should include:

## Layout

- Application shell
- Header
- Navigation
- Sidebar
- Page container
- Page header

## Navigation

- Navigation item
- Application navigation
- Breadcrumbs where useful
- Tabs

## Content

- Card
- Tile
- List
- Table
- Empty state
- Loading state
- Error state

## Forms

- Text input
- Number input
- Select
- Checkbox
- Date input
- Validation message
- Form section
- Form actions

## Actions

- Primary button
- Secondary button
- Destructive button
- Icon button
- Dropdown/menu

## Feedback

- Toast/notification
- Confirmation dialog
- Error message
- Loading indicator
- Success message

The goal is for future pages to compose these components rather than recreate them.

---

# 11. Responsive Design

The application should be usable on:

- Desktop
- Laptop
- Tablet
- Mobile

Desktop should be the primary design target, but mobile should not be treated as an afterthought.

The application shell should adapt rather than simply shrink.

For example:

```text
Desktop

[Sidebar] [Main Content]


Mobile

[Header]
[Main Content]
[Navigation/Menu]
```

Application tiles should automatically adjust their layout based on available width.

Tables should have an intentional mobile strategy rather than simply overflowing the viewport.

---

# 12. Work Items Frontend

The Work Items application should provide an interface for managing tasks/work items.

The frontend should support the existing API capabilities:

- View work items
- Filter work items by group
- Filter by type
- View an individual work item
- Create work items
- Edit work items
- Delete work items
- Add parents
- Remove parents

Potential page structure:

```text
Work
├── Overview
├── Work Items
│   ├── List
│   ├── Details
│   ├── Create
│   └── Edit
├── Groups
└── Tags
```

The UI should make relationships between work items understandable.

Parent/child relationships should be represented visually rather than exposing database terminology unnecessarily.

---

# 13. Banking Frontend

The Banking application should provide a unified interface for:

- Bank accounts
- Transactions
- Transaction items

Potential structure:

```text
Banking
├── Overview
├── Accounts
├── Transactions
└── Reports
```

## Banking Overview

The overview should eventually provide useful financial information at a glance.

Potential content:

- Account balances
- Recent transactions
- Account summary
- Spending information
- Relevant alerts

The overview should be designed so additional financial functionality can be added later without redesigning the application.

## Bank Accounts

Users should be able to:

- View accounts
- Create accounts
- Edit accounts
- Delete accounts
- View account details
- Navigate to transactions associated with an account

## Transactions

Users should be able to:

- View transactions
- Filter transactions
- Search transactions
- View transaction details
- Create transactions
- Edit transactions
- Delete transactions

The transaction interface should prioritize fast scanning.

---

# 14. Notes Frontend

Notes should provide a simple interface for creating and viewing personal notes.

Core functionality:

- List notes
- Search notes
- View note
- Create note
- Edit note
- Delete note

The interface should prioritize content over unnecessary controls.

---

# 15. Groups and Tags

Groups and tags are cross-cutting organizational concepts.

The frontend should make them reusable across relevant applications.

Groups should support:

- Group listing
- Group details
- Group membership where appropriate
- Related work items

Tags should support:

- Tag listing
- Tag creation
- Tag editing
- Tag deletion
- Assigning tags to supported entities
- Filtering by tag

The UI should avoid forcing users to understand the underlying relational database structure.

---

# 16. Household / Inventory

Moopel contains household-related functionality including:

- Fridges
- Fridge items
- Storage
- Storage items

These should be presented as practical inventory-management applications.

Possible structure:

```text
Household
├── Fridges
│   ├── Fridge list
│   ├── Fridge details
│   └── Items
│
└── Storage
    ├── Storage locations
    ├── Storage details
    └── Items
```

The frontend should make it easy to answer questions such as:

> "What do I have?"

and:

> "Where is it?"

---

# 17. Authentication and Session Management

The frontend must integrate with the existing authentication system.

Requirements include:

- Login
- Logout
- Authentication state
- Protected pages
- Session handling
- Unauthorized-state handling
- Expired-session handling

The frontend has already used protected browser storage for client-side state.

Authentication implementation should remain centralized rather than being duplicated across individual pages.

---

# 18. API Integration

The frontend should communicate with the Moopel API through dedicated client/service abstractions.

Components should preferably not contain raw HTTP implementation details.

Conceptually:

```text
Component
    ↓
Frontend Service
    ↓
API Client
    ↓
Moopel API
    ↓
SQL Server
```

For example:

```text
BankAccountsPage
       ↓
BankAccountService
       ↓
Moopel API
```

This separation makes the frontend easier to test and maintain.

---

# 19. API Error Handling

The frontend should provide consistent handling for API failures.

At minimum, distinguish:

- Validation errors
- Unauthorized
- Forbidden
- Not found
- Conflict
- Server errors
- Network failures

Users should receive actionable messages.

Avoid exposing raw exceptions, stack traces, or backend implementation details.

For example, instead of:

> `System.Net.Http.HttpRequestException...`

show:

> **Unable to load your bank accounts.**  
> Please try again.

---

# 20. Loading States

Every API-backed page should have an intentional loading state.

Avoid showing an empty page while data is being retrieved.

Examples:

- Skeleton cards
- Loading indicator
- Disabled action state
- Table loading state

The UI should distinguish:

```text
Loading
   ≠
Empty
   ≠
Error
```

This distinction is important throughout the application.

---

# 21. Empty States

Every collection page should have an intentional empty state.

Example:

```text
No bank accounts yet

Add your first bank account to start
managing your finances.

[ + Add Bank Account ]
```

Empty states should explain:

1. What is empty
2. Why the user might care
3. What they can do next

---

# 22. Forms and Validation

Forms should provide:

- Clear labels
- Appropriate input types
- Inline validation
- Required-field indicators
- Server validation handling
- Submit/loading states
- Cancel behavior
- Confirmation for destructive actions

Forms should avoid unnecessary fields and complexity.

The frontend should use the API as the authoritative source for server-side validation.

---

# 23. Destructive Actions

Destructive actions such as deletion should require intentional interaction.

Examples:

- Delete work item
- Delete transaction
- Delete note
- Delete bank account

For significant destructive actions, use a confirmation dialog.

The dialog should clearly identify what is being deleted.

---

# 24. Settings

Settings should provide centralized configuration for the user.

Potential sections include:

```text
Settings
├── Profile
├── Appearance
├── Theme
├── Preferences
└── Session / Security
```

The existing theme functionality should be integrated into the application shell so appearance changes affect the entire frontend consistently.

---

# 25. Themes and Visual Design

The frontend should have a centralized design system rather than page-specific styling.

The design system should define:

- Colors
- Typography
- Spacing
- Border radius
- Shadows
- Component states
- Breakpoints
- Form styling
- Navigation styling

Themes should be represented through shared design tokens.

Changing a theme should not require modifying individual components.

---

# 26. Accessibility

The frontend should target good accessibility practices.

Requirements include:

- Semantic HTML
- Keyboard navigation
- Visible focus states
- Appropriate labels
- Accessible form validation
- Meaningful button names
- Sufficient contrast
- Screen-reader-friendly status messages
- No interaction that requires a mouse exclusively

Interactive elements should behave consistently.

---

# 27. Performance

The frontend should avoid unnecessary API calls and rendering.

Requirements:

- Load data only when needed
- Avoid duplicate requests
- Reuse appropriate state
- Paginate large collections
- Avoid loading entire datasets unnecessarily
- Provide responsive feedback during requests

Large collections such as transactions and work items should not assume that every record can be loaded at once.

---

# 28. Routing

Routes should be predictable and human-readable.

Example:

```text
/
 /login
 /settings

 /work
 /work/items
 /work/items/{id}

 /banking
 /banking/accounts
 /banking/accounts/{id}
 /banking/transactions
 /banking/transactions/{id}

 /notes
 /notes/{id}

 /storage
 /storage/{id}

 /fridges
 /fridges/{id}
```

Routes should describe the user's conceptual location rather than backend controller names.

---

# 29. Frontend Architecture

A recommended conceptual architecture is:

```text
Moopel Frontend
│
├── Layout
│   ├── AppShell
│   ├── Header
│   └── Navigation
│
├── Components
│   ├── Cards
│   ├── Tiles
│   ├── Forms
│   ├── Tables
│   ├── Dialogs
│   └── Feedback
│
├── Pages
│   ├── Dashboard
│   ├── Work
│   ├── Banking
│   ├── Notes
│   ├── Storage
│   ├── Fridges
│   └── Settings
│
├── Services
│   ├── Authentication
│   ├── API clients
│   ├── Storage
│   └── Application state
│
└── Models
    └── Frontend/API DTOs
```

The exact project structure can evolve, but the separation between presentation, reusable components, API communication, and application state should remain.

---

# 30. Application Registry

Because the dashboard already uses concepts such as `AppNav` and `AppTiles`, Moopel should treat applications as structured frontend metadata.

Conceptually:

```text
Application
├── Name
├── Label
├── Description
├── Icon
├── Route
├── Availability
├── Badge
└── Navigation
```

This allows the dashboard and navigation to be generated consistently.

A future application should be able to register itself with the frontend without requiring significant changes to the dashboard.

---

# 31. State Management

State should be kept at the smallest appropriate scope.

### Component state

Use for:

- Form values
- Temporary UI state
- Dialog visibility
- Loading indicators

### Page/application state

Use for:

- Current filters
- Selected records
- Application-specific data

### Global state

Use only when information genuinely applies across the application.

Examples:

- Authentication state
- Current user
- Theme
- Global notifications

Avoid turning the entire application into one large global state container.

---

# 32. Testing Requirements

The frontend should have tests for critical behavior.

Tests should cover:

- Navigation
- Authentication guards
- API service behavior
- Form validation
- Error handling
- Application availability
- Dashboard rendering
- Important CRUD workflows

Critical user flows should be tested end-to-end where practical.

Example:

```text
Login
 ↓
Dashboard
 ↓
Banking
 ↓
Create account
 ↓
Account appears in list
```

---

# 33. Security Requirements

The frontend should assume that the API is the authority for security.

Requirements:

- Never trust client-side authorization alone
- Never expose secrets in frontend code
- Do not store sensitive credentials unnecessarily
- Handle expired authentication correctly
- Do not expose API errors containing sensitive information
- Use HTTPS in deployed environments

Client-side checks should improve UX, not replace server-side authorization.

---

# 34. UX Principles

The frontend should follow these principles.

### 1. Simple by default

Do not expose every possible option immediately.

### 2. Consistent

The same action should look and behave the same everywhere.

### 3. Fast to navigate

The user should be able to reach common functionality with minimal interaction.

### 4. Informative

The interface should always communicate:

- What is happening
- What went wrong
- What is available
- What the user can do next

### 5. Forgiving

Prevent accidental destructive actions and preserve user input where practical.

### 6. Modular

Each application should feel independent while still belonging to Moopel.

---

# 35. MVP Frontend Scope

The initial frontend MVP should prioritize the application shell and a small number of complete applications rather than building every API feature simultaneously.

## Phase 1 — Foundation

- Application shell
- Routing
- Authentication
- Dashboard
- Application tiles
- Global navigation
- Application navigation
- Theme system
- Shared components
- API client infrastructure
- Global error handling
- Loading/empty/error states

## Phase 2 — Core Applications

Prioritize:

1. Work Items
2. Banking
3. Notes
4. Groups
5. Tags

Each should have a complete user flow rather than partially implementing every module.

## Phase 3 — Household

- Fridges
- Fridge items
- Storage
- Storage items

## Phase 4 — Reporting and Advanced Features

- Reports
- Dashboard summaries
- Cross-application information
- Notifications/badges
- Additional analytics

---

# 36. Definition of Done

A frontend feature is considered complete when:

- The user can navigate to it naturally
- The page has a consistent layout
- Loading state exists
- Empty state exists where applicable
- Error state exists
- API errors are handled
- Validation exists where necessary
- Destructive operations are protected
- The page works responsively
- Keyboard navigation works
- The feature follows the shared design system
- Authentication/authorization behavior is correct
- The primary user flow has automated coverage where practical

---

# 37. Future Direction

The frontend should eventually evolve beyond simply exposing CRUD operations.

The long-term opportunity is to make Moopel useful because it **connects information together**.

For example:

```text
Work Item
   ├── Group
   ├── Tags
   ├── Notes
   └── Related information

Bank Account
   └── Transactions
          └── Tags / Categories

Storage Location
   └── Stored Items

Fridge
   └── Food Items
```

The dashboard could eventually become a true personal command center rather than simply an application launcher.

The frontend should therefore be designed around the idea that Moopel applications may eventually share information and surface useful relationships.

---

# 38. Success Criteria

The frontend will be successful when a user can open Moopel and immediately understand:

- What Moopel is
- What applications are available
- Where they currently are
- What they can do
- What requires attention

A successful frontend should make the complexity of the underlying API effectively invisible.

The user should experience Moopel as **one cohesive personal-management application**, not as a collection of API endpoints.

---

# 39. Open Product Questions

The following decisions remain undefined based on the current project context and should eventually be resolved:

1. What is the definitive visual identity/brand for Moopel?
2. Which applications should appear on the initial dashboard?
3. What is the exact relationship between Groups, Work Items, Notes, and Tags?
4. Which modules should have dedicated application-level navigation?
5. What information should appear on the dashboard beyond application tiles?
6. Should banking be purely manual or eventually support external account integration?
7. Should the frontend support multiple users with substantially different experiences?
8. What functionality should be available to administrators, if any?
9. What is the final mobile experience?
10. Which modules are MVP versus future functionality?

These should be treated as product decisions rather than frontend implementation details.
# E-Commerce Platform

## Introduction
This project is a comprehensive single-vendor e-commerce platform designed to deliver a seamless and engaging online shopping experience. Our goal is to build a robust e-commerce solution that enables customers to effortlessly browse products, manage their shopping carts, place orders, and share reviews, while providing administrators with powerful tools to manage inventory, process orders, and maintain platform excellence. The platform is motivated by the need to create a competitive e-commerce solution that incorporates modern features such as advanced product categorization, intelligent search functionality, personalized user experiences, and secure payment integration (supporting multiple payment methods including credit cards, PayPal, and digital

2. **Set Up the Database**:
   - Configure the database connection string in `appsettings.json`:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=EcommerceDb;Trusted_Connection=True;"
     }
     ```
   - Apply migrations to create or update the database:
     ```bash
     dotnet ef migrations add InitialCreate
     dotnet ef database update
     ```

## Git Branching Methodology
Our project follows a structured Git branching model to streamline development, testing, and deployment cycles. We use three primary branches:

- **production**: Contains the codebase deployed to the production environment. All changes here are live for end-users.
- **dev**: The active development branch where new features and fixes are merged before promotion to `master`.

### Specialized Branches
To organize development tasks, we use a folder-like structure for branches:

#### Feature and Routine Work Folders
Branches for non-urgent work are created from the `dev` branch:
- **features/**: For new features (e.g., adding AI recommendations).
- **bug-fix/**: For fixing bugs (e.g., cart calculation errors).
- **refactor/**: For code refactoring (e.g., optimizing order processing).

#### Urgent Work Folders
Branches for urgent tasks are created from the `master` branch:
- **hot-features/**: Urgent new features based on production code (e.g., adding a new payment gateway).
- **hot-fix/**: Urgent bug fixes affecting production (e.g., fixing a checkout failure).

### Branch Naming Conventions
Branches are named to reflect the type of work and the impacted area: `[type-of-work]/[sub-folder]/[brief-description]`

**Examples**:
- `features/reviews/add-review-photos`: Adding photo upload to reviews.
- `bug-fix/orders/fix-shipping-calculation`: Fixing shipping price calculation.
- `hot-fix/payments/resolve-paypal-error`: Urgent fix for PayPal payment failures.

**Subfolders**:
Branches are organized by the impacted area of the platform, aligned with our models:
- **auth/**: Authentication-related work (e.g., MFA, JWT).
- **buyer/**: Buyer features (e.g., cart, wishlist).
- **admin/**: Admin tools (e.g., review moderation).
- **products/**: Product-related features (e.g., AR visuals).
- **orders/**: Order processing (e.g., checkout, tracking).
- **reviews/**: Review system (e.g., helpful votes).
- **cart/**,....
This structure ensures clarity on the scope of each branch, aligning with the platform’s models (`Buyer`, `Product`, `Order`, `Review`, etc.).

### Branching Paradigm
We adopt a **modified Git Flow** approach:
- **Feature Development**: Create `feature/*` branches from `dev`, merge back to `dev` after code review.
- **Bug Fixes**: `bug-fix/*` branches follow the same flow as features.
- **Hotfixes**: Create `hot-fix/*` branches from `master`, apply urgent fixes, merge back to `master`, then propagate to `master` and `dev`.

This model supports parallel development of features like product reviews, payment integrations, and AR visuals while ensuring stable releases and quick hotfixes.

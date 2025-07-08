# Revenue Recognition System (RRS)

**Revenue Recognition System (RRS)** is a RESTful API built to solve the revenue recognition problem—a critical challenge in financial systems that ensures revenue is recorded accurately and in compliance with legal standards, especially for complex software contracts.

---

##  Features

### 1. Client Management
- **Add, update, and soft‑delete** individuals and companies  
- **Role‑based access**: only admins can edit or remove clients  

### 2. Software Licensing
- Track software products by **version**, **category**, and **description**  
- Support for **upfront contracts**  
- Apply **time‑bound discounts** and **loyalty offers**  

### 3. Contracts
- Create one‑time software contracts with:  
  - **Versioning**  
  - **Configurable payment windows** (upfront or installments)  
  - **Additional support years** (1–3)  
  - **Discount stacking** (max discount + loyalty)  
- Enforce **payment deadlines**, **installment support**, and **revenue recognition** upon full payment  

### 4. Revenue Calculation
- **Current revenue** based on actual payments received  
- **Predicted revenue** assuming all contracts are signed and renewed  
- **Dynamic currency conversion** using live exchange rates  

---

##  Technologies Used

- **Backend:** ASP.NET Core  
- **ORM:** Entity Framework Core  
- **API Documentation:** OpenAPI (Swagger)  
- **Database:** MySQL
- **Authentication:** JWT  
- **Testing:** xUnit, Moq  


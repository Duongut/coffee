# SRS - Cafe Management Web Application (Student Portfolio Version)

## 1. Tổng Quan Dự Án

### 1.1 Mô Tả Sản Phẩm
**Cafe Management Web Application** là một ứng dụng web demo cho quán cafe nhỏ, cho phép khách hàng tự order qua QR code và hệ thống quản lý đơn giản cho chủ quán. **Mục tiêu chính: Tạo portfolio project ấn tượng cho thực tập sinh.**

### 1.2 Mục Tiêu Dự Án
- **Portfolio Goal**: Demonstrate full-stack development skills
- **Technical Skills**: .NET, SQL Server, Web API, MVC, Authentication
- **Business Logic**: E-commerce workflow, payment integration (demo)
- **Development Approach**: Local development first, containerization later
- **Timeline**: 2-3 tháng (phù hợp với sinh viên)

### 1.3 Quy Mô Demo
- **Số bàn**: 10 bàn (demo scale)
- **Concurrent users**: 20-30 users (testing purposes)
- **Team size**: 1 sinh viên
- **Budget**: **HOÀN TOÀN MIỄN PHÍ** 🎓
- **Platform**: Web responsive

### 1.4 Công Nghệ Sử Dụng (Development Stack)
- **Backend**: .NET 8 Web MVC + Web API
- **Database**: **SQL Server LocalDB (Data Source=DESKTOP-V812IA7;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Name=vscode-mssql;Application Intent=ReadWrite;Command Timeout=30)
- **Cache**: In-Memory Caching (built-in)
- **Authentication**: Cookie Authentication
- **Version Control**: Git (local)
- **Testing**: Unit tests cơ bản
- **Future**: Docker containerization (giai đoạn 2)

## 2. Chức Năng Hệ Thống (Enhanced Portfolio Features)

### 2.1 Module Khách Hàng (Customer Experience)
#### **🎯 Core Features**
- **Dynamic QR Code System**: Mỗi bàn có QR riêng với table ID
- **Interactive Menu**: Filter theo category, search, favorites
- **Smart Shopping Cart**: 
  - Lưu cart trong session với auto-save
  - Tính toán tax và discount tự động
  - Estimation time cho order
- **Order Customization**: 
  - Notes cho từng món (ít đá, nhiều đường, etc.)
  - Size options và add-ons
  - Combo deals và recommendations

#### **✨ Advanced Features**
- **Table Service Mode**: 
  - Call waiter button (demo notification)
  - Request bill splitting
  - Table availability status
- **Mock Payment Options**: 
  - Multiple payment methods UI (Cash, Card, E-wallet)
  - Tip calculation với presets (10%, 15%, 20%)
  - Receipt generation (PDF download)
- **Order Tracking**: 
  - Real-time status updates với progress bar
  - Estimated completion time
  - Order history với reorder function
- **Customer Profile**: 
  - Guest checkout hoặc simple registration  
  - Favorite orders và quick reorder
  - Basic loyalty points system (demo)

#### **🎨 UX Enhancements**
- **Dark/Light Theme**: Toggle theme preference
- **Multi-language**: Vietnamese/English switch
- **Accessibility**: Screen reader support, keyboard navigation
- **Offline Mode**: Show cached menu khi mất internet

### 2.2 Module Quản Lý (Business Management)

#### **📊 Dashboard & Analytics**
- **Real-time Dashboard**: 
  - Live order count và revenue today
  - Popular items với charts
  - Table occupancy status
  - Daily/weekly sales trends
- **Business Intelligence**: 
  - Best-selling products analysis
  - Peak hours identification
  - Customer behavior insights
  - Revenue forecasting (basic trend analysis)

#### **🍕 Advanced Menu Management**
- **Product Management Pro**: 
  - Bulk import/export (CSV/Excel)
  - Product variants (size, temperature, milk type)
  - Ingredient tracking với allergen warnings
  - Recipe management với cost calculation
- **Dynamic Pricing**: 
  - Happy hour pricing rules
  - Seasonal menu items
  - Combo creator với discount logic
  - Promotional campaigns management

#### **📋 Order Management Plus**
- **Kitchen Display System**: 
  - Order queue với priority levels
  - Preparation time tracking
  - Staff task assignment
  - Order modification handling
- **Table Management**: 
  - Interactive floor plan
  - Table booking system (demo)
  - Occupancy time tracking
  - Table turnover optimization

#### **👥 Staff & Customer Management**
- **Staff Management**: 
  - Role-based access (Admin, Manager, Staff)
  - Shift scheduling system
  - Performance tracking (orders served, ratings)
  - Commission calculation cho staff
- **Customer Insights**: 
  - Customer database với order history
  - Loyalty program management
  - Feedback và rating system
  - Birthday offers và personal promotions

#### **💰 Financial Management**
- **Sales Reporting**: 
  - Daily/weekly/monthly reports
  - Profit margin analysis per product
  - Tax calculation và reporting
  - Export financial data (Excel, PDF)
- **Inventory Integration**: 
  - Stock level warnings
  - Automatic low-stock notifications
  - Supplier management (basic)
  - Cost analysis và pricing suggestions

### 2.3 Module Đặc Biệt (Portfolio Showcases)

#### **🚀 Technical Showcases**
- **API Integration Demos**: 
  - Weather widget (ảnh hưởng drink recommendations)
  - Currency converter cho tourist pricing
  - Social media feed integration (Instagram photos)
  - Map integration cho delivery zones
- **AI/ML Features (Mock)**: 
  - Recommendation engine based on order history
  - Demand forecasting cho inventory
  - Customer sentiment analysis từ reviews
  - Chatbot customer service (basic responses)

#### **📱 Modern Web Features**
- **Progressive Web App (PWA)**: 
  - Offline functionality
  - Push notifications (demo)
  - App-like experience on mobile
  - Home screen installation
- **Advanced UI Components**: 
  - Drag & drop menu reordering
  - Image gallery với zoom và swipe
  - Interactive charts và animations
  - Real-time chat support interface
- **Performance Features**: 
  - Image lazy loading và optimization
  - Caching strategies
  - Search với autocomplete và suggestions
  - Infinite scroll cho menu items

#### **🔐 Security & Admin Features**
- **Advanced Authentication**: 
  - Two-factor authentication (demo)
  - Social login options (Google, Facebook UI)
  - Password strength requirements
  - Session management với auto-logout
- **Audit & Compliance**: 
  - Activity logs cho all admin actions
  - Data export cho compliance
  - Backup và restore functionality
  - GDPR compliance features (data deletion)

### 2.4 Integration Demos (Mock Implementations)

#### **📧 Communication Systems**
- **Email Templates**: 
  - Order confirmation emails (HTML templates)
  - Promotional newsletters
  - Staff notifications
  - Customer birthday greetings
- **Notification System**: 
  - In-app notifications với toast messages
  - Admin alert system
  - Customer order status updates
  - Staff task assignments

#### **📊 Reporting & Export**
- **Advanced Reports**: 
  - Custom date range reports
  - Comparative analysis (this month vs last month)
  - Export multiple formats (PDF, Excel, CSV)
  - Scheduled reports (demo automation)
- **Data Visualization**: 
  - Interactive charts với Chart.js
  - Sales heatmaps
  - Customer journey visualization
  - Performance dashboards

### 2.5 Module Vẫn Bỏ Qua (Scope Control)
- ❌ **Real external integrations** (payment gateways, SMS services)
- ❌ **Complex inventory with suppliers** (ERP-level features)  
- ❌ **Advanced AI/ML implementation** (chỉ UI demo)
- ❌ **Multi-location management** (giữ single location)
- ❌ **Complex accounting integration** (QuickBooks, etc.)

### 2.6 Demo Data & Scenarios

#### **📋 Pre-loaded Demo Content**
- **Sample Menu**: 50+ items với đầy đủ categories
- **Mock Orders**: 100+ historical orders cho analytics
- **Customer Profiles**: 20+ sample customers với order history  
- **Staff Accounts**: Different roles để test permissions
- **Business Data**: 6 months of sales data cho reporting

#### **🎭 Demo Scenarios**
- **Rush Hour Simulation**: Multiple concurrent orders
- **Customer Journey**: From QR scan to payment completion
- **Manager Dashboard**: Real-time monitoring experience
- **Staff Workflow**: Order processing từ kitchen perspective
- **Business Analysis**: Monthly performance review

## 3. Kiến Trúc Phát Triển

### 3.1 Development Phases

**Phase 1: Local Development**
```
Local Development Environment
    ↓
Visual Studio / VS Code
├── CafeApp.Web (ASP.NET Core)
├── SQL Server LocalDB/Express
└── Local File Storage (images)
```

**Phase 2: Containerization (sau khi local hoàn thiện)**
```
Docker Environment
    ↓
Docker Compose
├── CafeApp.Web Container
├── SQL Server Container
└── Volume for Data Persistence
```

### 3.2 Project Structure
```
CafeApp/
├── CafeApp.Web/              # Main MVC Application
├── CafeApp.Core/             # Models + Interfaces
├── CafeApp.Data/             # Entity Framework
├── CafeApp.Services/         # Business Logic
├── CafeApp.Tests/            # Basic Unit Tests
├── README.md                 # Setup & demo instructions
├── Scripts/                  # Database setup scripts
└── Docker/                   # Docker files (Phase 2)
    ├── Dockerfile
    └── docker-compose.yml
```

### 3.3 Database Schema (Simplified)

```csharp
// Chỉ cần 5 entities chính
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;
    
    public Category Category { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; }
    public int TableNumber { get; set; } // Simple integer
    public string? CustomerName { get; set; }
    public OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<OrderItem> OrderItems { get; set; } = new();
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    
    public Order Order { get; set; }
    public Product Product { get; set; }
}

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
}

public enum OrderStatus
{
    Pending,
    Confirmed, 
    Preparing,
    Ready,
    Completed
}
```

## 4. Kế Hoạch Phát Triển (Student Timeline)

### 🎯 **PHASE 1: Local Development (5-6 tuần)**

### **Giai Đoạn 1.1: Foundation Setup (1 tuần)**
- [x] Setup .NET 8 project structure
- [ ] Setup Entity Framework với SQL Server LocalDB
- [ ] Create initial database schema
- [ ] Basic Identity authentication
- [ ] Test local database connection
- [ ] Setup basic project architecture

### **Giai Đoạn 1.2: Core Features (3-4 tuần)**

**Week 2: Database & Admin Foundation**
- [ ] Complete database migrations
- [ ] Admin authentication system
- [ ] Category management (CRUD)
- [ ] Basic admin dashboard
- [ ] Error handling và logging

**Week 3: Product Management**
- [ ] Product CRUD operations
- [ ] File upload for product images (wwwroot)
- [ ] Image management và validation
- [ ] Product-Category relationships
- [ ] Admin product management UI

**Week 4: Customer Features**
- [ ] Public menu display
- [ ] Shopping cart (session-based)
- [ ] Order creation và validation
- [ ] Order confirmation page
- [ ] Basic customer interface

**Week 5: Order Management**
- [ ] Admin order management
- [ ] Order status updates
- [ ] Order tracking for customers
- [ ] Simple reporting
- [ ] Demo payment flow

### **Giai Đoạn 1.3: UI/UX Polish (1 tuần)**
- [ ] Bootstrap 5 responsive design
- [ ] Mobile-friendly interface
- [ ] Loading states và error handling
- [ ] Basic CSS animations
- [ ] Cross-browser testing

### 🎯 **PHASE 2: Containerization (1-2 tuần)**

### **Giai Đoạn 2.1: Docker Setup**
- [ ] Create Dockerfile cho application
- [ ] Setup docker-compose với SQL Server
- [ ] Convert LocalDB connection to SQL Server container
- [ ] Test containerized application
- [ ] Volume mapping cho persistent data

### **Giai Đoạn 2.2: Production Ready**
- [ ] Environment configuration
- [ ] Container optimization
- [ ] Health checks
- [ ] Docker deployment documentation
- [ ] Production troubleshooting guide

## 5. Local Development Setup

### 5.1 Development Environment
- **IDE**: Visual Studio Community 2022 hoặc VS Code
- **Database**: SQL Server LocalDB (included with Visual Studio)
- **Alternative**: SQL Server Express (standalone)
- **Version Control**: Git (local repository)
- **Testing**: Local IIS Express

### 5.2 Database Configuration

**Local Development (appsettings.json):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CafeManagementDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

**Alternative SQL Server Express:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=CafeManagementDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### 5.3 Quick Start Commands (Local)

**Database Setup:**
```bash
# Create và apply migrations
dotnet ef migrations add InitialCreate
dotnet ef database update

# Seed sample data
dotnet run --seed-data
```

**Run Application:**
```bash
# Development mode
dotnet run

# Or with Visual Studio
F5 / Ctrl+F5
```

**Database Management:**
- **Visual Studio**: SQL Server Object Explorer
- **Standalone**: SQL Server Management Studio (SSMS)
- **Alternative**: Azure Data Studio

## 6. Technical Specifications (Local Development)

### 6.1 Development Requirements
- **OS**: Windows 10/11 (for LocalDB), hoặc Linux/Mac (với SQL Server Express)
- **RAM**: 8GB minimum
- **Storage**: 5GB free space
- **Network**: Chỉ cần cho package downloads

### 6.2 Local Database Features
- **LocalDB**: Lightweight, auto-start/stop
- **Express**: Full SQL Server features, manual management
- **Development**: Hot reload, migrations, seeding
- **Testing**: Isolated test database
- **Backup**: .bak file exports

### 6.3 Performance (Local)
- **Startup Time**: <5 seconds
- **Response Time**: <500ms (local network)
- **Concurrent Users**: 1 (development), 5-10 (local testing)
- **Database**: Instant queries với proper indexing

## 7. Testing Strategy (Local Development)

### 7.1 Local Testing Approach
- **Unit Tests**: xUnit với in-memory database
- **Integration Tests**: Test database với test data
- **Manual Testing**: Local browser testing
- **Database Testing**: SSMS query testing

### 7.2 Test Database Setup
```csharp
// Test configuration
public class TestDbContext : ApplicationDbContext
{
    public TestDbContext(DbContextOptions<TestDbContext> options) 
        : base(options) { }
}

// In-memory testing
services.AddDbContext<TestDbContext>(options =>
    options.UseInMemoryDatabase("TestDb"));
```

### 7.3 Local Demo Preparation
- **Sample Data**: Comprehensive seed data
- **Test Scenarios**: Pre-defined test flows
- **Browser Testing**: Multiple browsers locally
- **Performance**: Local performance profiling

## 8. Phase Transition Strategy

### 8.1 Local to Docker Transition

**Checklist trước khi chuyển Docker:**
- [ ] Application chạy stable trên local
- [ ] All features tested và working
- [ ] Database scripts sẵn sàng
- [ ] Configuration externalized
- [ ] Documentation hoàn thiện

**Migration Steps:**
1. **Backup local database**: Export data scripts
2. **Create Dockerfile**: Containerize application
3. **Setup docker-compose**: Add SQL Server service
4. **Update connection strings**: Point to container database
5. **Test containerized version**: Verify functionality
6. **Update documentation**: Docker-specific instructions

### 8.2 Benefits of This Approach

**Local Development First:**
- ✅ Faster development cycle
- ✅ Easier debugging
- ✅ No container complexity initially
- ✅ Familiar development environment
- ✅ Quick prototyping và iteration

**Docker Later:**
- ✅ Production-like environment
- ✅ Deployment flexibility
- ✅ Consistent environments
- ✅ Easy sharing và demonstration
- ✅ Professional deployment approach

## 9. Timeline Summary

### **Phase 1: Local Development (5-6 tuần)**
**Week 1:** Project setup, database, authentication
**Week 2:** Admin features, product management
**Week 3:** Customer features, shopping cart
**Week 4:** Order management, business logic
**Week 5:** UI polish, testing, optimization
**Week 6:** Documentation, demo preparation

### **Phase 2: Containerization (1-2 tuần)**
**Week 7:** Docker setup, container configuration
**Week 8:** Testing, optimization, final polish

### **Total: 7-8 tuần** với approach từ đơn giản đến phức tạp

## 10. Success Metrics

### 10.1 Local Development Success
- ✅ Application chạy locally không lỗi
- ✅ All CRUD operations working
- ✅ Database relationships correct
- ✅ Responsive UI trên multiple devices
- ✅ Basic security implemented
- ✅ Clean, maintainable code

### 10.2 Docker Migration Success
- ✅ Containerized application identical to local
- ✅ Database persistent trong containers
- ✅ Easy deployment với docker-compose
- ✅ Container health checks working
- ✅ Documentation updated

### 10.3 Portfolio Readiness
- **Technical Demo**: Show both local và containerized versions
- **Development Process**: Explain evolution from local to Docker
- **Problem Solving**: Demonstrate debugging in both environments
- **Best Practices**: Local development workflow + containerization

## 11. Cost Breakdown 💰

### **Total Budget: Chỉ cần máy tính** 🎉

**Development Phase:**
- **Development Tools**: Visual Studio Community (free)
- **Database**: SQL Server LocalDB (free with VS)
- **Version Control**: Git (free)
- **No external dependencies**: Everything local

**Containerization Phase:**
- **Docker Desktop**: Free for personal use
- **SQL Server Container**: Free Express edition
- **No hosting costs**: Local containers

**Hardware Requirements:**
- **Phase 1**: 4GB RAM minimum (local development)
- **Phase 2**: 8GB RAM recommended (Docker containers)
- **Storage**: 5GB (Phase 1) + 5GB (Phase 2)

## 12. Next Steps

### **Immediate Actions:**
1. **Setup Development Environment**: Install Visual Studio + SQL Server LocalDB
2. **Create Project Structure**: Initialize .NET solution
3. **Database First**: Setup Entity Framework với LocalDB
4. **Basic Authentication**: Implement login system
5. **First Feature**: Category management (simple CRUD)

### **Week 1 Goals:**
- Working local application
- Database connection established
- Admin login working
- Basic category CRUD
- Foundation cho remaining features

---

## 🎓 Kết Luận

Approach này optimize cho **development thực tế**:

### ✅ **Ưu Điểm:**
- **Simple Start**: Begin với familiar local development
- **Incremental Complexity**: Add Docker sau khi stable
- **Better Learning**: Understand fundamentals trước containerization
- **Faster Iteration**: No container overhead during development
- **Professional Progression**: Show evolution in technical approach

### 🎯 **Development Benefits:**
- **Debugging**: Easy debugging trong local environment
- **Performance**: Instant feedback cycle
- **Database**: Direct access với SSMS/Visual Studio
- **Testing**: Quick test iterations
- **Flexibility**: Easy configuration changes

### 📈 **Portfolio Story:**
*"Tôi bắt đầu với local development để focus vào business logic và core features. Sau khi application hoàn thiện và stable, tôi containerized nó với Docker để demonstrate deployment readiness và environment consistency."*

**Investment**: 50-70 giờ total (40h local + 10-30h Docker)
**Result**: Professional application với clear development progression 🚀
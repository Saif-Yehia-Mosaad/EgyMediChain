# EgyMediChain — Ministry / Super Admin Backend (.NET 8)

باك اند كامل لداشبورد الوزارة (Ministry / Super Admin) المطابق للشاشات اللي بعتها:
Overview, Registration Requests, Entities Management (Factories/Warehouses/Pharmacies),
Medicine & Batch Monitoring, Alerts & Public Scans, Admin & Audit.

## البنية (Clean Architecture مبسّطة)

```
EgyMediChain.sln
src/
  EgyMediChain.Domain/          -> Entities + Enums (POCOs فقط، بدون أي dependency)
  EgyMediChain.Infrastructure/   -> AppDbContext (EF Core + SQL Server) + DbSeeder (بيانات تجريبية كتيرة)
  EgyMediChain.Api/              -> Controllers + DTOs + JWT + Swagger
```

- **قاعدة البيانات**: SQL Server (متظبطة على السيرفر المحلي `DESKTOP-0MDMFGG\MSSQLSERVER04` عن طريق
  Windows Authentication) بيتبنى عن طريق **EF Core Migrations** فعلية — شوف قسم "ربط الداتابيز وعمل
  Migration" تحت لخطوات التشغيل أول مرة.
- **الداتا**: بمجرد ما تشغل المشروع أول مرة، `DbSeeder` بيحقن بيانات واقعية كتير (٣٢+ مستخدم، ٤٥ مصنع، ٤٠ مخزن،
  ٩٠ صيدلية، ١٥٠ باتش، ١٢٠ شحنة، ٤٥ تنبيه، ١٥٠ عملية مسح عام، ٦٠ طلب تسجيل بمستنداتها، ١٢٠ سجل Audit...) عشان
  الفرونت يبان بيه شكل حقيقي فورًا من غير ما تعمل حاجة.
- **Validation**: مقصود تكون خفيفة جدًا في الـ API (مفيش `[Required]` تقريبًا، كل حاجة nullable) عشان تقدر
  توصل الفرونت بالـ API بسرعة من غير ما الـ requests ترجع 400 لأي حقل ناقص وانت لسه بتظبط الشكل.

## طريقة التشغيل

> محتاج .NET 8 SDK مثبت على جهازك، ومحتاج اتصال إنترنت عادي لأول `dotnet restore` (عشان نزل الحزم من NuGet).
> ملحوظة: النسخة اللي جهزتها هنا اتكتبت كاملة الكود بس متعملهاش build في الـ sandbox لأن مفيش وصول لـ nuget.org هنا،
> فلازم تعمل `dotnet restore` / `dotnet run` عندك على جهازك أو في أي CI عنده إنترنت عادي.

> **مهم:** المشروع بيستخدم EF Core Migrations حقيقية، يعني قبل أول تشغيل لازم تعمل أول Migration
> مرة واحدة زي ما هو موضح في قسم "ربط الداتابيز وعمل Migration" تحت — لو شغلت المشروع من غير
> ما تعمل Migration الأول، `db.Database.Migrate()` هيرمي Exception لأنه مفيش جداول أصلاً.

```bash
cd EgyMediChain
dotnet restore
dotnet ef migrations add InitialCreate --project src/EgyMediChain.Infrastructure --startup-project src/EgyMediChain.Api --output-dir Persistence/Migrations
dotnet ef database update --project src/EgyMediChain.Infrastructure --startup-project src/EgyMediChain.Api
dotnet run --project src/EgyMediChain.Api
```

هيفتح على: `http://localhost:5080/swagger` (فيه كل الـ endpoints موثقة وقابلة للتجربة مباشرة من Swagger).

### تسجيل الدخول التجريبي — إيميل ثابت لكل Role

كل إيميل مكتوب فيه اسم الـ Role بنفسه عشان تعرف تدخل بيه على طول من غير لخبطة.
الباسورد نفسه لكل الحسابات: Passw0rd!123

| الإيميل | الباسورد | الـ Role |
|---|---|---|
| superadmin@egymedichain.com | Passw0rd!123 | SuperAdmin |
| ministryadmin@egymedichain.com | Passw0rd!123 | MinistryAdmin |
| ministryviewer@egymedichain.com | Passw0rd!123 | MinistryViewer |
| factoryuser@egymedichain.com | Passw0rd!123 | FactoryUser |
| warehouseuser@egymedichain.com | Passw0rd!123 | WarehouseUser |
| pharmacyuser@egymedichain.com | Passw0rd!123 | PharmacyUser |

```
POST /api/auth/login
{
  "email": "superadmin@egymedichain.com",
  "password": "Passw0rd!123"
}
```

الـ response بيرجع الـ role الحقيقي بتاع الإيميل ده (مثلاً "Role": "SuperAdmin")، فالفرونت
يقدر يوجّه المستخدم على الداشبورد المناسب له بناءً على القيمة دي مباشرة.

الحسابات الستة دي بتتضاف/تتحدّث تلقائيًا في كل مرة تشغل المشروع، حتى لو الداتابيز كانت
متعمولها seed قبل كده، فمش محتاج تعمل حاجة زيادة في الداتابيز.

فيه كمان حسابات عشوائية إضافية بإيميلات عشوائية في جدول SystemUsers — دول بس عشان يملّوا
الجداول والـ pagination في شاشة Admin & Audit، مش للاستخدام في تسجيل الدخول. استخدم الجدول
اللي فوق دايمًا للدخول بأي Role محدد.
هيرجعلك JWT (`token`) استخدمه في الفرونت كـ `Authorization: Bearer {token}`.

## خريطة الـ Endpoints (حسب الشاشات)

### Overview
- `GET /api/overview` → الكروت + Recent Registration Requests + Recent Alerts + Recent Batch Activity

### Registration Requests
- `GET /api/registration-requests?status=&search=&page=&pageSize=`
- `GET /api/registration-requests/counts` → عدد كل تبويب (Pending/NeedsMoreDocuments/Approved/Rejected/Cancelled)
- `GET /api/registration-requests/{id}` → تفاصيل كاملة (Account Info + Entity Info حسب النوع + Documents)
- `POST /api/registration-requests/{id}/approve`
- `POST /api/registration-requests/{id}/reject` → body: `{ "rejectionReason": "..." }`
- `POST /api/registration-requests/{id}/request-more-documents` → body: `{ "adminNotes": "...", "documentIdsNeedingReplacement": [1,2] }`
- `POST /api/registration-requests/documents/{documentId}/status` → body: `{ "status": "Complete|NeedsReplacement|Rejected", "rejectionReason": "..." }`

### Entities Management
**Factories**
- `GET /api/factories?search=&status=&page=&pageSize=`
- `GET /api/factories/{id}`
- `POST /api/factories/{id}/suspend` / `reactivate` / `set-inactive`
- `GET /api/factories/{id}/batches`

**Warehouses**
- `GET /api/warehouses?search=&status=&page=&pageSize=`
- `GET /api/warehouses/{id}`
- `POST /api/warehouses/{id}/suspend` / `reactivate` / `set-inactive`
- `GET /api/warehouses/{id}/inventory`
- `GET /api/warehouses/{id}/shipments`

**Pharmacies**
- `GET /api/pharmacies?search=&status=&page=&pageSize=`
- `GET /api/pharmacies/{id}`
- `POST /api/pharmacies/{id}/suspend` / `reactivate` / `set-inactive`
- `GET /api/pharmacies/{id}/inventory`
- `GET /api/pharmacies/{id}/shipments`

### Medicine & Batch Monitoring
- `GET /api/batches/summary` → الكروت (Total/InProduction/InSupplyChain/InWarehouses/InPharmacies/Quarantined/Recalled/OpenAlerts)
- `GET /api/batches?search=&factory=&batchStatus=&stage=&page=&pageSize=`
- `GET /api/batches/{id}` → Product Info + Batch Info + Unit Codes Summary + Shipments + Inventory Distribution + Related Alerts
- `POST /api/batches/{id}/freeze` → Quarantine + Blocked units/inventory + ComplianceIssue alert
- `POST /api/batches/{id}/create-recall-alert` → Recalled + Recalled units/inventory + Critical Recall alert

### Alerts & Public Scans
- `GET /api/alerts/counts` → عدد Open Alerts / Public Scan Logs / Recall Alerts (للتابات)
- `GET /api/alerts?status=&severity=&page=&pageSize=` (Open Alerts tab)
- `GET /api/alerts/recalls` (Recall Alerts tab)
- `GET /api/alerts/{id}`
- `POST /api/alerts/{id}/status` → body: `{ "status": "UnderReview|Resolved|Dismissed" }`
- `GET /api/alerts/public-scans?result=&page=&pageSize=`
- `GET /api/alerts/public-scans/{id}` → تفاصيل الـ Scan + بيانات الـ Unit Code لو موجود
- `POST /api/alerts/public-scans/{id}/create-alert` → body: `{ "message": "...", "severity": "High" }`

### Admin & Audit
- `GET /api/admin/users/summary` → Total/Active/Inactive/ActiveSessions
- `GET /api/admin/users?search=&role=&page=&pageSize=`
- `POST /api/admin/users` → Add Ministry Admin (body: FullName, Email, MobileNumber, NationalId, Role, TemporaryPassword)
- `POST /api/admin/users/{id}/activate` / `deactivate`
- `POST /api/admin/users/{id}/revoke-sessions`
- `GET /api/admin/audit-logs?from=&to=&page=&pageSize=` (read-only, no PUT/DELETE بالتصميم)
- `GET /api/admin/audit-logs/{id}`

## ربط الداتابيز وعمل Migration (EF Core Code-First)

المشروع دلوقتي بيستخدم **Migrations حقيقية** (مش `EnsureCreated`)، يعني زي بالظبط الأسلوب اللي انت متعود
عليه في TripMind/EgyMediChain. الـ `DbContext` (`AppDbContext`) موجود في مشروع **EgyMediChain.Infrastructure**،
والـ Startup Project هو **EgyMediChain.Api**.

### 1) الـ Connection String
المشروع دلوقتي متظبط بالفعل على **SQL Server** (مش SQLite)، مطابق للسيرفر اللي شغال عندك في
SSMS (`DESKTOP-0MDMFGG\MSSQLSERVER04`) بـ Windows Authentication. موجودة في
`src/EgyMediChain.Api/appsettings.json`:
```json
"ConnectionStrings": {
  "Default": "Server=DESKTOP-0MDMFGG\\MSSQLSERVER04;Database=EgyMediChainDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```
- `Trusted_Connection=True` معناها Windows Authentication (نفس الاختيار اللي شايفه في SSMS)، مش محتاج
  اليوزر/باسورد.
- `TrustServerCertificate=True` عشان الاتصال ميتعطلش لو الشهادة self-signed (نفس تصرف الـ checkbox
  "Trust server certificate" اللي فاتحه في SSMS).
- الداتابيز `EgyMediChainDb` **مش لازم تعملها إنت بإيدك** في SSMS — الـ Migration اللي جاي دلوقتي
  هيعملها تلقائيًا أول ما تعمل `database update`.

لو السيرفر اسمه مختلف عندك أو بتستخدم SQL Login (يوزر/باسورد) بدل Windows Authentication، غيّر السطر
ده بس، والباقي كله زي ما هو.

### 2) طريقة (أ) — لو بتستخدم Visual Studio (Package Manager Console)
1. من فوق: **Tools → NuGet Package Manager → Package Manager Console**
2. في أعلى الـ Console فيه Dropdown اسمه **Default project** → اختار **EgyMediChain.Infrastructure**
3. اتأكد إن الـ **Startup Project** بتاع الـ Solution هو **EgyMediChain.Api** (كليك يمين عليه → Set as Startup Project)
4. اكتب:
```powershell
Add-Migration InitialCreate -Project EgyMediChain.Infrastructure -StartupProject EgyMediChain.Api -OutputDir Persistence/Migrations
```
5. لو المشروع بنى تمام، تحت هيتكون فولدر `Persistence/Migrations` جوه `EgyMediChain.Infrastructure` فيه أول Migration.
6. بعد كده اعمل Update للداتابيز فعليًا:
```powershell
Update-Database -Project EgyMediChain.Infrastructure -StartupProject EgyMediChain.Api
```
هيتعمل الملف/الداتابيز فعليًا وتتحط فيه كل الجداول.

### 3) طريقة (ب) — لو بتستخدم .NET CLI (Terminal)
```bash
# مرة واحدة بس على جهازك (لو مش متثبتة):
dotnet tool install --global dotnet-ef

# من جوه فولدر EgyMediChain (اللي فيه الـ .sln):
dotnet ef migrations add InitialCreate \
  --project src/EgyMediChain.Infrastructure \
  --startup-project src/EgyMediChain.Api \
  --output-dir Persistence/Migrations

dotnet ef database update \
  --project src/EgyMediChain.Infrastructure \
  --startup-project src/EgyMediChain.Api
```

### 4) تشغيل المشروع بعد كده
لما تشغل `dotnet run --project src/EgyMediChain.Api` (أو F5 من Visual Studio)، الكود في `Program.cs`
بيعمل `db.Database.Migrate()` تلقائيًا أول ما السيرفر يفتح، يعني أي Migration جديدة عملتها هتتطبق
على الداتابيز لوحدها من غير ما تعمل حاجة يدوي، وبعدها الـ `DbSeeder` بيحقن الداتا التجريبية + الحسابات
الثابتة لكل Role.

### 5) لو عملت تعديل على أي Entity بعد كده (عمود جديد مثلاً)
كل مرة تعدل في أي كلاس جوه `EgyMediChain.Domain/Entities/Entities.cs`، لازم تعمل Migration جديدة بنفس
الخطوات فوق بس بإسم مختلف بدل `InitialCreate`، مثلاً:
```bash
dotnet ef migrations add AddPharmacyLoyaltyProgram --project src/EgyMediChain.Infrastructure --startup-project src/EgyMediChain.Api
dotnet ef database update --project src/EgyMediChain.Infrastructure --startup-project src/EgyMediChain.Api
```

> ملحوظة: لو كنت شغّلت نسخة قديمة من المشروع بـ SQLite قبل كده، متنساش تشيل أي reference ليها
> (زي ملف `egymedichain.db` لو كان اتعمل جنب الـ .exe) — مش هيأثر على SQL Server لكن نضّف المكان بس.

## تحديث: مطابقة شاشات Factory Portal / Warehouse Portal الفعلية

بعد ما بعتلي سكرين شوتس الشاشات الحقيقية، ضفت:
- حقول جديدة على `Factory`/`Warehouse`/`Pharmacy` (Code, Phone, Email) وحقول تفصيلية زيادة على
  `Factory` بس (Established Year, Total Production Lines, Main Production Types, Storage Types,
  Quality Certificates, Description, Registration Info snapshot) عشان تبان في تابات **Overview /
  Registration Info / Factory Details / Licenses / Documents** بتاعة Factory Profile.
- جدول جديد `EntityLicense` (License Type / Number / Issue Date / Expiry Date / Status) عشان تاب
  **Licenses** اللي بيعرض أكتر من رخصة (Manufacturing / GMP / Environmental / Fire Safety).
- Endpoints جديدة: `GET /api/factory-dashboard/{id}/shipments/summary`،
  `GET /api/factory-dashboard/{id}/alerts/{alertId}` (فيه "Impact on this Batch" panel)،
  `GET /api/warehouse-dashboard/{id}/inventory/summary`، `GET /api/warehouse-dashboard/{id}/shipments/summary`.
- **الحسابات التجريبية الستة اتربطت بأسماء محددة** بدل "أول صف" عشان تطابق الشاشات بالظبط:
  `factoryuser@` → **EIPICO Factory** (FAC-2024-021)، `warehouseuser@` → **Cairo Medical Storage**
  (WH-CAI-001)، `pharmacyuser@` → **Alexandria Drug Store**. الحساب ده بقى فيه 9 باتشات، شحنات،
  وتنبيهات مخصوصة مطابقة تقريبًا لأرقام الـ demo اللي في السكرين شوتس (BAT-2024-001 → 009،
  TRF-2024-0170 → 0178، ALERT-2024-0083 → 0091).

⚠️ **لازم Migration جديدة** بعد التحديث ده (فيه أعمدة وجدول جديد):
```bash
dotnet ef migrations add AlignFactoryWarehousePortals --project src/EgyMediChain.Infrastructure --startup-project src/EgyMediChain.Api
dotnet ef database update --project src/EgyMediChain.Infrastructure --startup-project src/EgyMediChain.Api
```

## Factory / Warehouse / Pharmacy Dashboards (Operational Portals)

بالإضافة لداشبورد الوزارة، الباك اند دلوقتي فيه 3 بورتالات تشغيلية كاملة مطابقة للـ specs بتاعت
المصنع/المخزن/الصيدلية، بنفس فلسفة الـ API (nullable-heavy, low validation, business rules الأساسية
مطبقة فعليًا زي الـ cold-chain block والـ status transitions).

كل بورتال Scoped بـ `{id}` في الـ route (مش من الـ JWT) عشان يفضل بسيط - الفرونت ياخد الـ `entityId`
من response بتاع `/api/auth/login` (شوف جدول الحسابات فوق) ويستخدمه مباشرة.

### Factory Dashboard — `/api/factory-dashboard/{factoryId}`
- `GET /overview`
- `GET /batches` , `GET /batches/{batchId}`
- `POST /batches` (Create Batch — بيعمل reuse لـ MedicineProduct لو GTIN موجود، غير كده بيعمله جديد. `saveAsDraft: true|false`)
- `POST /batches/{batchId}/generate-codes` (لازم BatchStatus = Registered)
- `POST /batches/{batchId}/mark-ready` (لازم BatchStatus = CodesGenerated)
- `POST /batches/{batchId}/cancel-draft` (لازم BatchStatus = Draft)
- `GET /shipments` , `GET /shipments/{shipmentId}`
- `POST /shipments` (Dispatch to Warehouse — بيتأكد من الـ cold chain والكمية المتاحة قبل الإرسال)
- `GET /alerts` (read-only)
- `GET /profile`

### Warehouse Dashboard — `/api/warehouse-dashboard/{warehouseId}`
- `GET /overview`
- `GET /shipments/incoming` , `GET /shipments/outgoing` , `GET /shipments/{shipmentId}`
- `POST /shipments/{shipmentId}/receive` (Accepted/PartiallyAccepted/Rejected — بيحدث الـ InventoryStock تلقائيًا)
- `GET /inventory` , `GET /inventory/{inventoryId}`
- `POST /inventory/{inventoryId}/dispatch-to-pharmacy` (بيتأكد من الـ cold chain والكمية المتاحة)
- `POST /inventory/{inventoryId}/move-to-quarantine` (لازم `HasQuarantineArea = true`)
- `POST /report-issue`
- `GET /alerts` (read-only)
- `GET /profile`

### Pharmacy Dashboard — `/api/pharmacy-dashboard/{pharmacyId}`
- `GET /overview`
- `GET /shipments` , `GET /shipments/{shipmentId}`
- `POST /shipments/{shipmentId}/receive`
- `GET /inventory` , `GET /inventory/{inventoryId}`
- `POST /report-issue`
- `GET /alerts` (read-only)
- `GET /profile`

### قواعد مطبقة فعليًا (مش بس نصوص في الـ spec)
- كل الـ endpoints التشغيلية (Create/Generate/Dispatch/Receive/Quarantine/Report) بترجع **409 Conflict**
  لو الكيان (Factory/Warehouse/Pharmacy) مش `Active` — بالظبط زي الـ Suspended rule في الـ specs.
- الـ Cold Chain rule متطبقة فعليًا: أي Dispatch أو Receive لباتش `RequiresColdChain = true` لجهة
  `HasColdStorage != true` بيترفض بـ 409 ورسالة واضحة.
- الـ Batch lifecycle (`Draft → Registered → CodesGenerated → ReadyForWarehouseDispatch →
  PartiallyDispatched/FullyDispatched`) متطبق بالترتيب، كل Action بيتأكد من الـ status الحالي الأول.
- Dispatch من المصنع أو المخزن بيتأكد إن الكمية المطلوبة ≤ الكمية المتاحة فعليًا قبل ما يعمل الشحنة.
- المصنع/المخزن/الصيدلية **ما عندهمش** أي endpoint لـ Resolve/Dismiss Alert أو Create Recall —
  دي فضلت حصرًا في `/api/alerts` بتاعة الوزارة.




1. **CORS مفتوح بالكامل** في `Program.cs` (`SetIsOriginAllowed(_ => true)`) عشان تقدر توصل من React
   من أي بورت من غير ما تتعب في الإعدادات. لما تقفل المشروع على production غيّرها لقايمة origins محددة.
2. **الـ Enums بترجع كـ string** في الـ JSON (مش أرقام) عشان تبقى سهلة القراءة في الفرونت مباشرة
   (`"FactoryStatus": "Active"` مش `0`).
3. **UnitCode جدول فيه عينة بسيطة فقط** (مش ملايين الصفوف) — أرقام الملخص (Total/Generated/InWarehouse/...)
   محفوظة كحقول جاهزة على الـ `Batch` نفسه زي ما اتفقنا في التصميم، فالـ Batch Details drawer هيبان مباشرة
   من غير ما نحمّل آلاف الصفوف.
4. **داشبورد الوزارة نفسه ما فيهوش** أي endpoint لـ Create Batch / Generate Unit Codes / Create Shipment /
   Receive Shipment / Edit Inventory / Scan QR — العمليات دي موجودة بس في بورتالات المصنع/المخزن/الصيدلية
   التشغيلية (شوف قسم "Factory / Warehouse / Pharmacy Dashboards" فوق)، مش في `/api/*` بتاعة الوزارة.
5. الباسوردات بتتعمل لها Hash بـ BCrypt (work factor 12) قبل ما تتخزن، والباسورد الأصلي مبيترجعش في أي response.

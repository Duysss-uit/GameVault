# Implementation Hint: User Authentication UI

## 🎯 Goal
Add a user-facing authentication system so users can log in, register, and manage their personal game collections.

## 📍 Current State

### ✅ What You Already Have:
1. **Backend JWT Authentication** is configured in `GameVault.ApiService/Program.cs`:
   ```csharp
   builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(options => { ... });
   ```

2. **API Protection**: Your `UserGamesController` has `[Authorize]` attribute:
   ```csharp
   [Authorize]
   [ApiController]
   [Route("api/[controller]")]
   public class UserGamesController : ControllerBase
   ```

3. **User identification**: Controller uses `User.FindFirst(ClaimTypes.NameIdentifier)?.Value`

### ❌ What's Missing:
- No Login page/component
- No Registration page/component  
- No way for users to actually authenticate
- No token storage on the client side
- No logout functionality
- No authentication state management in Blazor

## 🤔 What Needs to Be Built?

### Frontend Components:
1. Login page
2. Register page
3. User profile/account page (optional but recommended)
4. Authentication state provider
5. Token storage mechanism

### Backend Endpoints (if not already exist):
1. POST /api/auth/login
2. POST /api/auth/register
3. POST /api/auth/refresh (for token refresh)

## 💡 Implementation Hints

### Step 1: Check If Auth Endpoints Exist

**Look for**: Do you have an `AuthController` in your API project?

**Where to check**: `GameVault.ApiService/Controllers/`

**If not there**: You'll need to create authentication endpoints first (backend work).

### Step 2: Create Authentication DTOs

You'll need models for:

```csharp
// Login request
public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

// Register request  
public class RegisterRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

// Auth response
public class AuthResponse
{
    public string Token { get; set; }
    public string Username { get; set; }
    public DateTime ExpiresAt { get; set; }
}
```

**Hint**: Put these in a new file like `GameVault.Web/DTOs/AuthDTOs.cs`

### Step 3: Create Authentication Service

Create `IAuthenticationService` and `AuthenticationService`:

**What it should do**:
- Login (send credentials, receive token)
- Register (send user info)
- Logout (clear stored token)
- Get current token
- Check if user is authenticated

**Token Storage Options**:
1. **Browser localStorage** - persists across sessions
2. **Browser sessionStorage** - only for current session
3. **Cookies** - can be HttpOnly for security

**Hint for Blazor + localStorage**: You'll need JSInterop

```csharp
// Example pattern (not complete code)
public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;
    
    public async Task<AuthResponse> LoginAsync(string username, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/auth/login", 
            new LoginRequest { Username = username, Password = password });
            
        if (response.IsSuccessStatusCode)
        {
            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
            // Store token in localStorage
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", authResponse.Token);
            return authResponse;
        }
        // Handle error
    }
}
```

### Step 4: Create Login Page

Create `GameVault.Web/Components/Pages/Login.razor`

**What it needs**:
- Username input field
- Password input field  
- Login button
- Link to Register page
- Error message display
- Loading state during login

**Blazor pattern**:
```razor
@page "/login"
@inject IAuthenticationService AuthService
@inject NavigationManager Navigation

<div class="row justify-content-center">
    <div class="col-md-6">
        <div class="card">
            <div class="card-body">
                <h3>Login to GameVault</h3>
                
                @if (!string.IsNullOrEmpty(errorMessage))
                {
                    <div class="alert alert-danger">@errorMessage</div>
                }
                
                <EditForm Model="loginModel" OnValidSubmit="HandleLogin">
                    <DataAnnotationsValidator />
                    
                    <div class="mb-3">
                        <label>Username</label>
                        <InputText @bind-Value="loginModel.Username" class="form-control" />
                        <ValidationMessage For="() => loginModel.Username" />
                    </div>
                    
                    <div class="mb-3">
                        <label>Password</label>
                        <InputText @bind-Value="loginModel.Password" type="password" class="form-control" />
                        <ValidationMessage For="() => loginModel.Password" />
                    </div>
                    
                    <button type="submit" class="btn btn-primary" disabled="@isLoading">
                        @if (isLoading)
                        {
                            <span class="spinner-border spinner-border-sm"></span>
                        }
                        Login
                    </button>
                </EditForm>
                
                <p class="mt-3">
                    Don't have an account? <a href="/register">Register here</a>
                </p>
            </div>
        </div>
    </div>
</div>

@code {
    private LoginModel loginModel = new();
    private string errorMessage = "";
    private bool isLoading = false;
    
    private async Task HandleLogin()
    {
        try
        {
            isLoading = true;
            errorMessage = "";
            
            var result = await AuthService.LoginAsync(
                loginModel.Username, 
                loginModel.Password
            );
            
            if (result != null)
            {
                // Redirect to home after successful login
                Navigation.NavigateTo("/");
            }
        }
        catch (Exception ex)
        {
            errorMessage = "Login failed. Please check your credentials.";
        }
        finally
        {
            isLoading = false;
        }
    }
    
    public class LoginModel
    {
        [Required]
        public string Username { get; set; } = "";
        
        [Required]
        public string Password { get; set; } = "";
    }
}
```

### Step 5: Add Authentication to HttpClient

Your API calls need to include the JWT token in the Authorization header.

**Where**: Look at `GameApiService.cs` where you have `_httpClient`

**What to add**: 
```csharp
private async Task<string> GetTokenAsync()
{
    // Get token from localStorage via JSInterop
    return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
}

// Before each API call:
var token = await GetTokenAsync();
if (!string.IsNullOrEmpty(token))
{
    _httpClient.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", token);
}
```

**Better approach**: Use `HttpClient` delegating handlers to automatically add token to all requests.

### Step 6: Update Navigation Menu

Add authentication-aware navigation in `NavMenu.razor`:

```razor
<AuthorizeView>
    <Authorized>
        <!-- Show when logged in -->
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="profile">
                <span class="bi bi-person-fill" aria-hidden="true"></span> Profile
            </NavLink>
        </div>
        <div class="nav-item px-3">
            <button class="nav-link btn btn-link" @onclick="HandleLogout">
                <span class="bi bi-box-arrow-right" aria-hidden="true"></span> Logout
            </button>
        </div>
    </Authorized>
    <NotAuthorized>
        <!-- Show when not logged in -->
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="login">
                <span class="bi bi-box-arrow-in-right" aria-hidden="true"></span> Login
            </NavLink>
        </div>
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="register">
                <span class="bi bi-person-plus-fill" aria-hidden="true"></span> Register
            </NavLink>
        </div>
    </NotAuthorized>
</AuthorizeView>
```

### Step 7: Create Custom AuthenticationStateProvider

Blazor uses `AuthenticationStateProvider` to track auth state.

**Where**: Create `GameVault.Web/Services/CustomAuthStateProvider.cs`

**What it does**:
- Reads token from storage
- Parses JWT to get user claims
- Provides authentication state to components

**Key methods to implement**:
```csharp
public class CustomAuthStateProvider : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await GetTokenFromStorage();
        
        if (string.IsNullOrEmpty(token))
        {
            // Not authenticated
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
        
        // Parse token and create claims
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);
        
        return new AuthenticationState(user);
    }
    
    public void NotifyUserAuthentication(string token)
    {
        // After login, notify Blazor that auth state changed
        var claims = ParseClaimsFromJwt(token);
        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);
        
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(user))
        );
    }
    
    public void NotifyUserLogout()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(anonymous))
        );
    }
}
```

**JWT Token Parsing Hint**: JWT tokens are Base64 encoded. The payload is the middle part (between two dots).

### Step 8: Register Services in Program.cs

In `GameVault.Web/Program.cs`:

```csharp
// Add authentication services
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
```

### Step 9: Protect Routes

For pages that require authentication, add `@attribute [Authorize]`:

```razor
@page "/games/add"
@attribute [Authorize]
```

If user tries to access without being logged in, they'll be redirected to login.

## 🧪 Testing Your Implementation

### Test Cases:

1. **Login with valid credentials** → Should redirect to home, show user menu
2. **Login with invalid credentials** → Should show error message
3. **Access protected page without login** → Should redirect to login
4. **Logout** → Should clear token, redirect to login, show public menu
5. **Token expiration** → Should handle gracefully (show login again)
6. **Register new user** → Should create account and login
7. **Refresh page while logged in** → Should stay logged in (token persists)

## 🔒 Security Considerations

### DO:
- ✅ Use HTTPS in production
- ✅ Store tokens securely (HttpOnly cookies are most secure)
- ✅ Implement token expiration
- ✅ Hash passwords on backend (never store plain text)
- ✅ Validate input (prevent SQL injection, XSS)
- ✅ Use strong password requirements

### DON'T:
- ❌ Store passwords in localStorage/sessionStorage
- ❌ Send passwords in GET requests  
- ❌ Log sensitive information
- ❌ Trust client-side validation alone
- ❌ Use predictable user IDs

## 🎨 UI/UX Enhancements

### Nice touches:
- "Remember Me" checkbox (longer token expiration)
- "Forgot Password" link
- Social login (Google, Facebook, etc.)
- Email verification
- Profile picture upload
- User preferences
- Account settings page

### Error Messages:
Make them helpful:
- ❌ "Error" 
- ✅ "Incorrect username or password"
- ❌ "401 Unauthorized"
- ✅ "Your session has expired. Please log in again."

## 📚 Resources to Learn

- JWT (JSON Web Tokens): How they work
- Blazor Authentication & Authorization
- ASP.NET Core Identity (if you want full user management)
- Browser storage (localStorage vs sessionStorage vs cookies)
- C# Claims-based authentication

## ✅ Success Criteria

You'll know it works when:
- [ ] Users can register for an account
- [ ] Users can login with credentials
- [ ] Token is stored and persisted across page refreshes
- [ ] Protected pages require authentication
- [ ] Navigation menu shows correct items based on auth state
- [ ] Users can logout and token is cleared
- [ ] API calls include authentication token
- [ ] Expired tokens are handled gracefully

## 🚀 Extension Ideas

After basic auth works:
1. **Password reset flow** (email verification)
2. **Two-factor authentication** (2FA)
3. **OAuth/Social login** (Google, GitHub, etc.)
4. **Role-based authorization** (admin vs regular user)
5. **Session management** (view active sessions, logout all)
6. **Account deletion** (with confirmation)
7. **Activity log** (login history)

---

**Pro Tip**: Start with a simple username/password login. Get that working end-to-end before adding fancy features like OAuth or 2FA.

**Security First**: Even for a learning project, practice good security habits. It builds good instincts!

Good luck! 🔐

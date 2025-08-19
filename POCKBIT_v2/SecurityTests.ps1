# SecurityTests.ps1 - Pruebas automatizadas de seguridad para POCKBIT_v2
param(
    [string]$BaseUrl = "https://localhost:44344"  # Cambia este puerto por el tuyo
)

Write-Host "🔒 Iniciando pruebas de seguridad para POCKBIT_v2..." -ForegroundColor Green
Write-Host "Target: $BaseUrl" -ForegroundColor Yellow
Write-Host "Fecha: $(Get-Date)" -ForegroundColor Gray

# Función para probar SQL Injection
function Test-SQLInjection {
    param([string]$url)
    
    $payloads = @(
        "' OR '1'='1",
        "admin' OR '1'='1' --",
        "' OR 1=1 --",
        "admin'--",
        "1' UNION SELECT 1,2,3--",
        "'; DROP TABLE users--"
    )
    
    Write-Host "`n📋 Testing SQL Injection (TC_SEC_01)..." -ForegroundColor Cyan
    $vulnerabilities = 0
    
    foreach ($payload in $payloads) {
        try {
            # Intentar en página de login (ajusta la ruta según tu proyecto)
            $loginData = @{
                Email = $payload
                Password = "test123"
            }
            
            $response = Invoke-RestMethod -Uri "$url/Account/Login" -Method POST -Body $loginData -ErrorAction SilentlyContinue
            
            # Verificar si el login fue exitoso (indica vulnerabilidad)
            if ($response -match "welcome|dashboard|admin|success") {
                Write-Host "❌ CRÍTICO: SQL Injection exitoso con: $payload" -ForegroundColor Red
                $vulnerabilities++
            } else {
                Write-Host "✅ BLOQUEADO: $payload" -ForegroundColor Green
            }
        }
        catch {
            # Si hay error, generalmente significa que está bloqueado
            if ($_.Exception.Message -match "400|403|500") {
                Write-Host "✅ BLOQUEADO: $payload (Error HTTP)" -ForegroundColor Green
            } else {
                Write-Host "⚠️  ERROR de conexión con: $payload" -ForegroundColor Yellow
            }
        }
        
        Start-Sleep -Milliseconds 1000  # Pausa entre requests
    }
    
    return $vulnerabilities
}

# Función para probar XSS
function Test-XSS {
    param([string]$url)
    
    $payloads = @(
        "<script>alert('XSS')</script>",
        "<img src=x onerror=alert('XSS')>",
        "<svg onload=alert('XSS')>",
        "javascript:alert('XSS')",
        "<iframe src='javascript:alert(`"XSS`")'>",
        "';alert('XSS');//"
    )
    
    Write-Host "`n📋 Testing XSS (TC_SEC_02)..." -ForegroundColor Cyan
    $vulnerabilities = 0
    
    foreach ($payload in $payloads) {
        try {
            # Probar en diferentes endpoints según tu aplicación
            $endpoints = @(
                "/Home/Contact",
                "/Account/Register", 
                "/Admin/Comments"  # Ajusta según tus rutas
            )
            
            foreach ($endpoint in $endpoints) {
                $testData = @{
                    Message = $payload
                    Name = "TestUser"
                    Email = "test@test.com"
                }
                
                try {
                    $response = Invoke-RestMethod -Uri "$url$endpoint" -Method POST -Body $testData -ErrorAction SilentlyContinue
                    
                    # Verificar si el script se ejecutaría (payload no escapado)
                    if ($response -match "<script>|javascript:|onerror=|onload=") {
                        Write-Host "❌ VULNERABLE: XSS en $endpoint con: $payload" -ForegroundColor Red
                        $vulnerabilities++
                    }
                }
                catch {
                    # Continuar con el siguiente endpoint
                }
            }
            
            Write-Host "✅ PROTEGIDO contra: $payload" -ForegroundColor Green
        }
        catch {
            Write-Host "✅ PROTEGIDO: $payload (Rechazado)" -ForegroundColor Green
        }
        
        Start-Sleep -Milliseconds 500
    }
    
    return $vulnerabilities
}

# Función para probar headers de seguridad
function Test-SecurityHeaders {
    param([string]$url)
    
    Write-Host "`n📋 Testing Security Headers..." -ForegroundColor Cyan
    
    try {
        $response = Invoke-WebRequest -Uri $url -UseBasicParsing
        $headers = $response.Headers
        
        # Verificar headers importantes
        $securityHeaders = @{
            "X-Frame-Options" = "Protección contra clickjacking"
            "X-XSS-Protection" = "Protección XSS del navegador"
            "X-Content-Type-Options" = "Prevenir MIME sniffing"
            "Strict-Transport-Security" = "Forzar HTTPS"
            "Content-Security-Policy" = "Política de seguridad de contenido"
        }
        
        foreach ($header in $securityHeaders.Keys) {
            if ($headers.ContainsKey($header)) {
                Write-Host "✅ $header presente: $($securityHeaders[$header])" -ForegroundColor Green
            } else {
                Write-Host "⚠️  $header ausente: $($securityHeaders[$header])" -ForegroundColor Yellow
            }
        }
    }
    catch {
        Write-Host "❌ Error al verificar headers: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Función para generar reporte
function Generate-Report {
    param([int]$sqlVulns, [int]$xssVulns)
    
    $reportPath = "SecurityTestReport_$(Get-Date -Format 'yyyyMMdd_HHmmss').txt"
    
    $report = @"
========================================
REPORTE DE PRUEBAS DE SEGURIDAD POCKBIT_v2
========================================
Fecha: $(Get-Date)
Target: $BaseUrl

RESULTADOS:
-----------
SQL Injection (TC_SEC_01): $(if($sqlVulns -eq 0){"✅ PASS"}else{"❌ FAIL ($sqlVulns vulnerabilidades)"})
XSS Protection (TC_SEC_02): $(if($xssVulns -eq 0){"✅ PASS"}else{"❌ FAIL ($xssVulns vulnerabilidades)"})

TOTAL VULNERABILIDADES: $($sqlVulns + $xssVulns)

$(if(($sqlVulns + $xssVulns) -eq 0){"🎉 APLICACIÓN SEGURA"}else{"⚠️  REQUIERE ATENCIÓN INMEDIATA"})

========================================
"@

    $report | Out-File -FilePath $reportPath -Encoding UTF8
    Write-Host "`n📄 Reporte guardado en: $reportPath" -ForegroundColor Cyan
    
    return $reportPath
}

# EJECUCIÓN PRINCIPAL
# ===================

# Verificar que el servidor esté corriendo
try {
    $testConnection = Invoke-WebRequest -Uri $BaseUrl -UseBasicParsing -TimeoutSec 5
    Write-Host "✅ Servidor accesible" -ForegroundColor Green
}
catch {
    Write-Host "❌ ERROR: No se puede conectar a $BaseUrl" -ForegroundColor Red
    Write-Host "   Asegúrate de que tu aplicación esté corriendo" -ForegroundColor Yellow
    exit 1
}

# Ejecutar todas las pruebas
$sqlVulnerabilities = Test-SQLInjection -url $BaseUrl
$xssVulnerabilities = Test-XSS -url $BaseUrl
Test-SecurityHeaders -url $BaseUrl

# Generar reporte final
$reportFile = Generate-Report -sqlVulns $sqlVulnerabilities -xssVulns $xssVulnerabilities

# Resumen final
Write-Host "`n" + "="*50 -ForegroundColor White
Write-Host "🎯 RESUMEN FINAL" -ForegroundColor White -BackgroundColor DarkBlue
Write-Host "="*50 -ForegroundColor White

if (($sqlVulnerabilities + $xssVulnerabilities) -eq 0) {
    Write-Host "🎉 ¡FELICIDADES! Tu aplicación pasó todas las pruebas de seguridad" -ForegroundColor Green
} else {
    Write-Host "⚠️  Se encontraron $($sqlVulnerabilities + $xssVulnerabilities) vulnerabilidades que requieren atención" -ForegroundColor Red
    Write-Host "📋 Revisa el reporte detallado: $reportFile" -ForegroundColor Yellow
}

Write-Host "`n🔍 Para más pruebas, ejecuta: .\SecurityTests.ps1 -BaseUrl 'tu_url_aqui'" -ForegroundColor Cyan
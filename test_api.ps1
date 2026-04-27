$base = "http://localhost:5046/api/todo"

function Invoke-Api($method, $uri, $body = $null) {
    try {
        $params = @{ Uri = $uri; Method = $method; UseBasicParsing = $true }
        if ($body) { $params.Body = $body; $params.ContentType = "application/json" }
        $r = Invoke-WebRequest @params
        Write-Host "$method $uri => $($r.StatusCode)" -ForegroundColor Green
        if ($r.Content) { $r.Content | ConvertFrom-Json | ConvertTo-Json -Compress }
    } catch {
        $resp = $_.Exception.Response
        if ($resp) {
            $s = $resp.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($s)
            $b = $reader.ReadToEnd()
            Write-Host "$method $uri => $($resp.StatusCode.Value__) $b" -ForegroundColor Red
        } else {
            Write-Host "$method $uri => ERROR: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    Write-Host ""
}

# 1. GET all
Invoke-Api GET "$base"

# 2. POST invalid (empty name => validation error)
Invoke-Api POST "$base" (@{name=""} | ConvertTo-Json)

# 3. POST valid
Invoke-Api POST "$base" (@{name="Test Mediator"} | ConvertTo-Json)

# 4. GET all again
Invoke-Api GET "$base"

# 5. GET active
Invoke-Api GET "$base`?filter=active"

# 6. PATCH complete first todo
$all = Invoke-WebRequest -Uri "$base" -Method GET -UseBasicParsing | Select-Object -ExpandProperty Content | ConvertFrom-Json
if ($all -and $all.Count -gt 0) {
    $id = $all[0].id
    Invoke-Api PATCH "$base/$id/complete"
    Invoke-Api GET "$base`?filter=completed"
}

# 7. PUT update
if ($all -and $all.Count -gt 0) {
    $id = $all[0].id
    Invoke-Api PUT "$base/$id" (@{name="Updated name"} | ConvertTo-Json)
}

# 8. PATCH update-all
Invoke-Api PATCH "$base/update-all" (@{isCompleted=$true} | ConvertTo-Json)
Invoke-Api GET "$base"

# 9. DELETE completed
Invoke-Api DELETE "$base/completed"
Invoke-Api GET "$base"

# 10. DELETE todo
if ($all -and $all.Count -gt 0) {
    $id = $all[0].id
    Invoke-Api DELETE "$base/$id"
    Invoke-Api GET "$base"
}


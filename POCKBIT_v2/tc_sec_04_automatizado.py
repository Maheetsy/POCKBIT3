#!/usr/bin/env python3
"""
TC_SEC_04 - Prueba Automatizada de Seguridad de APIs
Verifica que las APIs estén protegidas contra acceso no autorizado
Ejecuta: python tc_sec_04_automatizado.py
"""

import requests
import json
import time
from datetime import datetime
from typing import Dict, List, Tuple

class TestTC_SEC_04:
    def __init__(self):
        self.resultados = {
            'pruebas_totales': 0,
            'pruebas_exitosas': 0,
            'apis_vulnerables': [],
            'apis_seguras': [],
            'errores_encontrados': []
        }
        
        # Configuración de APIs de prueba
        self.apis_test = [
            {
                'nombre': 'API Usuarios',
                'url': 'https://jsonplaceholder.typicode.com/users',
                'metodo': 'GET',
                'requiere_auth': True
            },
            {
                'nombre': 'API Posts',
                'url': 'https://jsonplaceholder.typicode.com/posts',
                'metodo': 'GET',
                'requiere_auth': True
            },
            {
                'nombre': 'API Comments',
                'url': 'https://jsonplaceholder.typicode.com/comments',
                'metodo': 'POST',
                'requiere_auth': True,
                'data': {'name': 'test', 'email': 'test@test.com', 'body': 'test comment'}
            }
        ]
        
        # Tokens de prueba
        self.tokens_prueba = {
            'valido': 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...',
            'invalido': 'Bearer token_invalido_123',
            'malformado': 'InvalidToken123',
            'vacio': '',
            'null': None
        }
    
    def ejecutar_prueba_completa(self):
        """Ejecuta toda la prueba TC_SEC_04 automáticamente"""
        print("🔒 INICIANDO PRUEBA TC_SEC_04: Verificación de Seguridad de APIs")
        print("=" * 70)
        
        # Paso 1: Configurar entorno de prueba
        print("⚙️  Paso 1: Configurando entorno de pruebas...")
        self._configurar_entorno()
        
        # Paso 2: Probar acceso sin token
        print("🚫 Paso 2: Probando acceso sin autenticación...")
        self._probar_acceso_sin_token()
        
        # Paso 3: Probar con tokens inválidos
        print("❌ Paso 3: Probando con tokens inválidos...")
        self._probar_tokens_invalidos()
        
        # Paso 4: Probar con tokens alterados
        print("🔧 Paso 4: Probando con tokens alterados...")
        self._probar_tokens_alterados()
        
        # Paso 5: Verificar códigos de respuesta
        print("📊 Paso 5: Verificando códigos de respuesta...")
        self._verificar_codigos_respuesta()
        
        # Paso 6: Mostrar resultados
        print("📋 Paso 6: Generando reporte de seguridad...")
        self._mostrar_resultados()
        
        return self._evaluar_seguridad()
    
    def _configurar_entorno(self):
        """Configura el entorno de pruebas"""
        print("   🔧 Configurando URLs y tokens de prueba...")
        print(f"   📝 APIs configuradas: {len(self.apis_test)}")
        print(f"   🔑 Tokens de prueba: {len(self.tokens_prueba)}")
        print("   ✅ Entorno configurado correctamente")
    
    def _probar_acceso_sin_token(self):
        """Prueba acceso a APIs sin token de autorización"""
        print("   🚫 Probando acceso sin token...")
        
        for api in self.apis_test:
            self.resultados['pruebas_totales'] += 1
            
            try:
                # Hacer petición sin headers de autorización
                if api['metodo'] == 'GET':
                    response = requests.get(api['url'], timeout=10)
                elif api['metodo'] == 'POST':
                    response = requests.post(api['url'], 
                                          json=api.get('data', {}), 
                                          timeout=10)
                
                # Verificar que la respuesta sea 401 o 403
                if response.status_code in [401, 403]:
                    print(f"   ✅ {api['nombre']}: PROTEGIDA (Status: {response.status_code})")
                    self.resultados['apis_seguras'].append(api['nombre'])
                    self.resultados['pruebas_exitosas'] += 1
                else:
                    print(f"   ⚠️  {api['nombre']}: VULNERABLE (Status: {response.status_code})")
                    self.resultados['apis_vulnerables'].append({
                        'api': api['nombre'],
                        'problema': 'Permite acceso sin token',
                        'status': response.status_code
                    })
                    
            except requests.RequestException as e:
                print(f"   ❌ Error probando {api['nombre']}: {str(e)}")
                self.resultados['errores_encontrados'].append(f"{api['nombre']}: {str(e)}")
    
    def _probar_tokens_invalidos(self):
        """Prueba con diferentes tokens inválidos"""
        print("   ❌ Probando con tokens inválidos...")
        
        tokens_invalidos = ['invalido', 'malformado', 'vacio']
        
        for api in self.apis_test[:2]:  # Solo primeras 2 APIs para no saturar
            for token_tipo in tokens_invalidos:
                self.resultados['pruebas_totales'] += 1
                
                try:
                    headers = {}
                    if token_tipo != 'vacio':
                        headers['Authorization'] = self.tokens_prueba[token_tipo]
                    
                    if api['metodo'] == 'GET':
                        response = requests.get(api['url'], headers=headers, timeout=10)
                    elif api['metodo'] == 'POST':
                        response = requests.post(api['url'], 
                                              headers=headers,
                                              json=api.get('data', {}), 
                                              timeout=10)
                    
                    if response.status_code in [401, 403]:
                        print(f"   ✅ {api['nombre']} + Token {token_tipo}: RECHAZADO correctamente")
                        self.resultados['pruebas_exitosas'] += 1
                    else:
                        print(f"   ⚠️  {api['nombre']} + Token {token_tipo}: ACEPTA token inválido")
                        self.resultados['apis_vulnerables'].append({
                            'api': api['nombre'],
                            'problema': f'Acepta token {token_tipo}',
                            'status': response.status_code
                        })
                        
                except requests.RequestException as e:
                    print(f"   ❌ Error con token {token_tipo}: {str(e)}")
                    self.resultados['errores_encontrados'].append(f"Token {token_tipo}: {str(e)}")
    
    def _probar_tokens_alterados(self):
        """Prueba con tokens alterados/manipulados"""
        print("   🔧 Probando con tokens alterados...")
        
        # Generar tokens alterados
        token_base = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9"
        tokens_alterados = [
            f"Bearer {token_base}.payload_alterado.signature",
            f"Bearer {token_base[:-5]}XXXXX",  # Alterar los últimos caracteres
            "Bearer " + "X" * 50,  # Token completamente falso
        ]
        
        for api in self.apis_test[:1]:  # Solo 1 API para ejemplo
            for i, token_alterado in enumerate(tokens_alterados, 1):
                self.resultados['pruebas_totales'] += 1
                
                try:
                    headers = {'Authorization': token_alterado}
                    
                    if api['metodo'] == 'GET':
                        response = requests.get(api['url'], headers=headers, timeout=10)
                    elif api['metodo'] == 'POST':
                        response = requests.post(api['url'], 
                                              headers=headers,
                                              json=api.get('data', {}), 
                                              timeout=10)
                    
                    if response.status_code in [401, 403]:
                        print(f"   ✅ Token alterado {i}: RECHAZADO correctamente")
                        self.resultados['pruebas_exitosas'] += 1
                    else:
                        print(f"   ⚠️  Token alterado {i}: ACEPTADO (PROBLEMA GRAVE)")
                        self.resultados['apis_vulnerables'].append({
                            'api': api['nombre'],
                            'problema': f'Acepta token alterado {i}',
                            'status': response.status_code
                        })
                        
                except requests.RequestException as e:
                    print(f"   ❌ Error con token alterado {i}: {str(e)}")
                    self.resultados['errores_encontrados'].append(f"Token alterado {i}: {str(e)}")
    
    def _verificar_codigos_respuesta(self):
        """Verifica que los códigos de respuesta sean los correctos"""
        print("   📊 Verificando códigos de respuesta HTTP...")
        
        codigos_esperados = {
            'sin_token': [401, 403],
            'token_invalido': [401, 403],
            'token_expirado': [401]
        }
        
        # Simular diferentes escenarios
        escenarios = [
            {'nombre': 'Sin Authorization Header', 'headers': {}},
            {'nombre': 'Header vacío', 'headers': {'Authorization': ''}},
            {'nombre': 'Solo "Bearer"', 'headers': {'Authorization': 'Bearer'}},
        ]
        
        for escenario in escenarios:
            self.resultados['pruebas_totales'] += 1
            
            try:
                api = self.apis_test[0]  # Usar primera API
                
                if api['metodo'] == 'GET':
                    response = requests.get(api['url'], headers=escenario['headers'], timeout=10)
                else:
                    response = requests.post(api['url'], 
                                          headers=escenario['headers'],
                                          json=api.get('data', {}), 
                                          timeout=10)
                
                if response.status_code in [401, 403]:
                    print(f"   ✅ {escenario['nombre']}: Status {response.status_code} (Correcto)")
                    self.resultados['pruebas_exitosas'] += 1
                else:
                    print(f"   ⚠️  {escenario['nombre']}: Status {response.status_code} (Incorrecto)")
                    self.resultados['apis_vulnerables'].append({
                        'api': api['nombre'],
                        'problema': f"Código incorrecto para {escenario['nombre']}",
                        'status': response.status_code
                    })
                    
            except requests.RequestException as e:
                print(f"   ❌ Error verificando {escenario['nombre']}: {str(e)}")
                self.resultados['errores_encontrados'].append(f"{escenario['nombre']}: {str(e)}")
    
    def _mostrar_resultados(self):
        """Muestra el reporte final de seguridad"""
        print("\n" + "=" * 70)
        print("🔒 REPORTE FINAL DE SEGURIDAD - TC_SEC_04")
        print("=" * 70)
        
        print(f"🧪 Pruebas ejecutadas: {self.resultados['pruebas_totales']}")
        print(f"✅ Pruebas exitosas: {self.resultados['pruebas_exitosas']}")
        print(f"❌ Pruebas fallidas: {self.resultados['pruebas_totales'] - self.resultados['pruebas_exitosas']}")
        
        porcentaje_exito = (self.resultados['pruebas_exitosas'] / self.resultados['pruebas_totales'] * 100) if self.resultados['pruebas_totales'] > 0 else 0
        print(f"📊 Porcentaje de éxito: {porcentaje_exito:.1f}%")
        
        print(f"\n🛡️  APIs Seguras: {len(self.resultados['apis_seguras'])}")
        for api in self.resultados['apis_seguras']:
            print(f"   ✅ {api}")
        
        print(f"\n⚠️  APIs Vulnerables: {len(self.resultados['apis_vulnerables'])}")
        for vuln in self.resultados['apis_vulnerables']:
            print(f"   🚨 {vuln['api']}: {vuln['problema']} (Status: {vuln['status']})")
        
        if self.resultados['errores_encontrados']:
            print(f"\n❌ Errores encontrados: {len(self.resultados['errores_encontrados'])}")
            for error in self.resultados['errores_encontrados']:
                print(f"   🔴 {error}")
        
        print("\n📋 VERIFICACIONES REALIZADAS:")
        print("   🔍 Acceso sin token de autorización")
        print("   🔍 Tokens inválidos y malformados")
        print("   🔍 Tokens alterados/manipulados")
        print("   🔍 Códigos de respuesta HTTP correctos")
        print("   🔍 Headers de autorización incorrectos")
    
    def _evaluar_seguridad(self):
        """Evalúa el nivel de seguridad general"""
        porcentaje_exito = (self.resultados['pruebas_exitosas'] / self.resultados['pruebas_totales'] * 100) if self.resultados['pruebas_totales'] > 0 else 0
        
        # Criterios para evaluar seguridad
        apis_vulnerables = len(self.resultados['apis_vulnerables'])
        
        if porcentaje_exito >= 80 and apis_vulnerables == 0:
            nivel = "🟢 EXCELENTE"
            mensaje = "Todas las APIs están bien protegidas"
        elif porcentaje_exito >= 60 and apis_vulnerables <= 2:
            nivel = "🟡 ACEPTABLE"
            mensaje = "Seguridad buena con algunas mejoras menores"
        elif porcentaje_exito >= 40:
            nivel = "🟠 MEJORABLE"
            mensaje = "Seguridad insuficiente, requiere atención"
        else:
            nivel = "🔴 CRÍTICO"
            mensaje = "Seguridad muy deficiente, requiere acción inmediata"
        
        print(f"\n🎯 EVALUACIÓN FINAL DE SEGURIDAD: {nivel}")
        print(f"   {mensaje}")
        print("=" * 70)
        
        return porcentaje_exito >= 60


def main():
    """Función principal que ejecuta toda la prueba"""
    print("🚀 Ejecutando TC_SEC_04 - Prueba de Seguridad API")
    print("   Este script verifica la protección de APIs contra acceso no autorizado\n")
    
    test = TestTC_SEC_04()
    seguro = test.ejecutar_prueba_completa()
    
    if seguro:
        print("\n🎉 ¡APIs correctamente protegidas!")
        print("   Las verificaciones de seguridad fueron exitosas.")
    else:
        print("\n⚠️  Se encontraron problemas de seguridad.")
        print("   Revisa las APIs vulnerables reportadas.")
    
    print("\n💡 RECOMENDACIONES:")
    print("   • Siempre verificar tokens de autorización")
    print("   • Retornar 401 para tokens inválidos")
    print("   • Retornar 403 para acceso sin permisos")
    print("   • Validar formato y firma de tokens JWT")
    print("   • Implementar rate limiting")
    
    return seguro


if __name__ == "__main__":
    main()
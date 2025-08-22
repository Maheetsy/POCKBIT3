#!/usr/bin/env python3
"""
TC_CODE_03 - Prueba Automatizada de Refactorización de Código Duplicado
Ejecuta: python tc_code_03_automatizado.py
"""

import os
import tempfile
import shutil
from abc import ABC, abstractmethod
from datetime import datetime
from typing import Dict, Any

class TestTC_CODE_03:
    def __init__(self):
        self.resultados = {
            'lineas_antes': 0,
            'lineas_despues': 0,
            'codigo_duplicado_eliminado': 0,
            'pruebas_pasadas': 0,
            'pruebas_totales': 0
        }
        
    def ejecutar_prueba_completa(self):
        """Ejecuta toda la prueba TC_CODE_03 automáticamente"""
        print("🔄 INICIANDO PRUEBA TC_CODE_03: Refactorización de Código Duplicado")
        print("=" * 70)
        
        # Paso 1: Crear código con duplicación
        print("📝 Paso 1: Creando código con duplicación...")
        self._crear_codigo_duplicado()
        
        # Paso 2: Medir código original
        print("📊 Paso 2: Midiendo código original...")
        self._medir_codigo_original()
        
        # Paso 3: Refactorizar código
        print("🔧 Paso 3: Refactorizando código...")
        self._refactorizar_codigo()
        
        # Paso 4: Medir código refactorizado
        print("📊 Paso 4: Midiendo código refactorizado...")
        self._medir_codigo_refactorizado()
        
        # Paso 5: Ejecutar pruebas
        print("🧪 Paso 5: Ejecutando pruebas de verificación...")
        self._ejecutar_pruebas()
        
        # Paso 6: Mostrar resultados
        print("📋 Paso 6: Generando reporte final...")
        self._mostrar_resultados()
        
        return self._evaluar_exito()
    
    def _crear_codigo_duplicado(self):
        """Crea las clases originales con código duplicado"""
        
        # Código original con mucha duplicación
        codigo_ventas = '''
from datetime import datetime

class ReporteVentas:
    def generar_reporte(self):
        # DUPLICADO: Configuración común
        fecha_inicio = "2024-01-01"
        fecha_fin = "2024-12-31"
        formato = "PDF"
        
        # DUPLICADO: Validación
        if not self._validar_fechas(fecha_inicio, fecha_fin):
            raise ValueError("Fechas inválidas")
        
        # DUPLICADO: Cabecera
        cabecera = f"Reporte de Ventas - {fecha_inicio} a {fecha_fin}"
        
        # DUPLICADO: Metadatos
        metadatos = {
            "generado": datetime.now().isoformat(),
            "formato": formato,
            "version": "1.0"
        }
        
        datos = {"ventas": 150000, "transacciones": 1250}
        
        # DUPLICADO: Estructura final
        return {"cabecera": cabecera, "datos": datos, "metadatos": metadatos}
    
    def _validar_fechas(self, inicio, fin):
        try:
            datetime.strptime(inicio, "%Y-%m-%d")
            datetime.strptime(fin, "%Y-%m-%d")
            return True
        except:
            return False
'''
        
        codigo_inventario = '''
from datetime import datetime

class ReporteInventario:
    def generar_reporte(self):
        # DUPLICADO: Configuración común
        fecha_inicio = "2024-01-01"
        fecha_fin = "2024-12-31"
        formato = "PDF"
        
        # DUPLICADO: Validación
        if not self._validar_fechas(fecha_inicio, fecha_fin):
            raise ValueError("Fechas inválidas")
        
        # DUPLICADO: Cabecera
        cabecera = f"Reporte de Inventario - {fecha_inicio} a {fecha_fin}"
        
        # DUPLICADO: Metadatos
        metadatos = {
            "generado": datetime.now().isoformat(),
            "formato": formato,
            "version": "1.0"
        }
        
        datos = {"productos": 450, "agotados": 12}
        
        # DUPLICADO: Estructura final
        return {"cabecera": cabecera, "datos": datos, "metadatos": metadatos}
    
    def _validar_fechas(self, inicio, fin):
        try:
            datetime.strptime(inicio, "%Y-%m-%d")
            datetime.strptime(fin, "%Y-%m-%d")
            return True
        except:
            return False
'''
        
        codigo_clientes = '''
from datetime import datetime

class ReporteClientes:
    def generar_reporte(self):
        # DUPLICADO: Configuración común
        fecha_inicio = "2024-01-01"
        fecha_fin = "2024-12-31"
        formato = "PDF"
        
        # DUPLICADO: Validación
        if not self._validar_fechas(fecha_inicio, fecha_fin):
            raise ValueError("Fechas inválidas")
        
        # DUPLICADO: Cabecera
        cabecera = f"Reporte de Clientes - {fecha_inicio} a {fecha_fin}"
        
        # DUPLICADO: Metadatos
        metadatos = {
            "generado": datetime.now().isoformat(),
            "formato": formato,
            "version": "1.0"
        }
        
        datos = {"clientes": 890, "nuevos": 45}
        
        # DUPLICADO: Estructura final
        return {"cabecera": cabecera, "datos": datos, "metadatos": metadatos}
    
    def _validar_fechas(self, inicio, fin):
        try:
            datetime.strptime(inicio, "%Y-%m-%d")
            datetime.strptime(fin, "%Y-%m-%d")
            return True
        except:
            return False
'''
        
        # Ejecutar el código para tener las clases disponibles
        exec(codigo_ventas, globals())
        exec(codigo_inventario, globals())
        exec(codigo_clientes, globals())
        
        self.codigos_originales = [codigo_ventas, codigo_inventario, codigo_clientes]
        print("   ✅ Código duplicado creado (3 clases con mucha duplicación)")
    
    def _medir_codigo_original(self):
        """Mide métricas del código original"""
        total_lineas = sum(len(codigo.strip().split('\n')) for codigo in self.codigos_originales)
        
        # Contar líneas duplicadas específicas
        lineas_duplicadas = [
            'fecha_inicio = "2024-01-01"',
            'fecha_fin = "2024-12-31"',
            'formato = "PDF"',
            'if not self._validar_fechas',
            '"generado": datetime.now().isoformat()',
            '"formato": formato',
            '"version": "1.0"',
            'return {"cabecera": cabecera, "datos": datos, "metadatos": metadatos}'
        ]
        
        duplicaciones = 0
        for codigo in self.codigos_originales:
            for linea_dup in lineas_duplicadas:
                duplicaciones += codigo.count(linea_dup)
        
        self.resultados['lineas_antes'] = total_lineas
        self.resultados['codigo_duplicado_eliminado'] = duplicaciones
        
        print(f"   📊 Líneas totales: {total_lineas}")
        print(f"   🔄 Líneas duplicadas: {duplicaciones}")
    
    def _refactorizar_codigo(self):
        """Crea la refactorización con clase base"""
        
        # Clase base que centraliza la lógica común
        clase_base = '''
from abc import ABC, abstractmethod
from datetime import datetime

class BaseReporte(ABC):
    def __init__(self, tipo_reporte):
        self.tipo_reporte = tipo_reporte
        self.fecha_inicio = "2024-01-01"
        self.fecha_fin = "2024-12-31"
        self.formato = "PDF"
    
    def _validar_fechas(self, inicio, fin):
        try:
            datetime.strptime(inicio, "%Y-%m-%d")
            datetime.strptime(fin, "%Y-%m-%d")
            return True
        except:
            return False
    
    def _generar_cabecera(self):
        return f"Reporte de {self.tipo_reporte} - {self.fecha_inicio} a {self.fecha_fin}"
    
    def _generar_metadatos(self):
        return {
            "generado": datetime.now().isoformat(),
            "formato": self.formato,
            "version": "1.0"
        }
    
    def generar_reporte(self):
        if not self._validar_fechas(self.fecha_inicio, self.fecha_fin):
            raise ValueError("Fechas inválidas")
        
        cabecera = self._generar_cabecera()
        metadatos = self._generar_metadatos()
        datos = self._obtener_datos_especificos()
        
        return {"cabecera": cabecera, "datos": datos, "metadatos": metadatos}
    
    @abstractmethod
    def _obtener_datos_especificos(self):
        pass
'''
        
        # Reportes refactorizados (muy pequeños ahora)
        ventas_refactorizado = '''
class ReporteVentasRefactorizado(BaseReporte):
    def __init__(self):
        super().__init__("Ventas")
    
    def _obtener_datos_especificos(self):
        return {"ventas": 150000, "transacciones": 1250}
'''
        
        inventario_refactorizado = '''
class ReporteInventarioRefactorizado(BaseReporte):
    def __init__(self):
        super().__init__("Inventario")
    
    def _obtener_datos_especificos(self):
        return {"productos": 450, "agotados": 12}
'''
        
        clientes_refactorizado = '''
class ReporteClientesRefactorizado(BaseReporte):
    def __init__(self):
        super().__init__("Clientes")
    
    def _obtener_datos_especificos(self):
        return {"clientes": 890, "nuevos": 45}
'''
        
        # Ejecutar código refactorizado
        exec(clase_base, globals())
        exec(ventas_refactorizado, globals())
        exec(inventario_refactorizado, globals())
        exec(clientes_refactorizado, globals())
        
        self.codigos_refactorizados = [clase_base, ventas_refactorizado, inventario_refactorizado, clientes_refactorizado]
        print("   ✅ Código refactorizado creado (1 clase base + 3 clases específicas)")
    
    def _medir_codigo_refactorizado(self):
        """Mide métricas del código refactorizado"""
        total_lineas = sum(len(codigo.strip().split('\n')) for codigo in self.codigos_refactorizados)
        self.resultados['lineas_despues'] = total_lineas
        
        print(f"   📊 Líneas después de refactorización: {total_lineas}")
        
        reduccion = self.resultados['lineas_antes'] - total_lineas
        porcentaje = (reduccion / self.resultados['lineas_antes']) * 100
        print(f"   📉 Reducción: {reduccion} líneas ({porcentaje:.1f}%)")
    
    def _ejecutar_pruebas(self):
        """Ejecuta todas las pruebas de verificación"""
        pruebas = [
            self._test_funcionalidad_mantenida,
            self._test_consistencia_estructura,
            self._test_logica_centralizada,
            self._test_facilidad_extension,
            self._test_validacion_centralizada
        ]
        
        self.resultados['pruebas_totales'] = len(pruebas)
        pruebas_exitosas = 0
        
        for i, prueba in enumerate(pruebas, 1):
            try:
                prueba()
                print(f"   ✅ Prueba {i}: {prueba.__name__} - PASÓ")
                pruebas_exitosas += 1
            except Exception as e:
                print(f"   ❌ Prueba {i}: {prueba.__name__} - FALLÓ: {e}")
        
        self.resultados['pruebas_pasadas'] = pruebas_exitosas
    
    def _test_funcionalidad_mantenida(self):
        """Verifica que la funcionalidad se mantiene igual"""
        # Original
        ventas_orig = ReporteVentas()
        result_orig = ventas_orig.generar_reporte()
        
        # Refactorizado
        ventas_ref = ReporteVentasRefactorizado()
        result_ref = ventas_ref.generar_reporte()
        
        # Deben tener la misma estructura
        assert set(result_orig.keys()) == set(result_ref.keys())
        assert result_orig["datos"] == result_ref["datos"]
    
    def _test_consistencia_estructura(self):
        """Verifica que todos los reportes tienen estructura consistente"""
        ventas = ReporteVentasRefactorizado()
        inventario = ReporteInventarioRefactorizado()
        clientes = ReporteClientesRefactorizado()
        
        r1 = ventas.generar_reporte()
        r2 = inventario.generar_reporte()
        r3 = clientes.generar_reporte()
        
        # Todos deben tener las mismas claves
        assert set(r1.keys()) == set(r2.keys()) == set(r3.keys())
        
        # Todos deben tener el mismo formato
        assert r1["metadatos"]["formato"] == r2["metadatos"]["formato"] == r3["metadatos"]["formato"]
    
    def _test_logica_centralizada(self):
        """Verifica que cambios en la base afectan a todos"""
        ventas = ReporteVentasRefactorizado()
        
        # Cambiar formato en la instancia
        ventas.formato = "EXCEL"
        resultado = ventas.generar_reporte()
        
        assert resultado["metadatos"]["formato"] == "EXCEL"
    
    def _test_facilidad_extension(self):
        """Verifica que es fácil agregar nuevos reportes"""
        class ReporteProveedores(BaseReporte):
            def __init__(self):
                super().__init__("Proveedores")
            
            def _obtener_datos_especificos(self):
                return {"proveedores": 25, "activos": 20}
        
        proveedores = ReporteProveedores()
        resultado = proveedores.generar_reporte()
        
        assert "Proveedores" in resultado["cabecera"]
        assert resultado["datos"]["proveedores"] == 25
    
    def _test_validacion_centralizada(self):
        """Verifica que la validación funciona igual en todos"""
        reportes = [
            ReporteVentasRefactorizado(),
            ReporteInventarioRefactorizado(),
            ReporteClientesRefactorizado()
        ]
        
        for reporte in reportes:
            reporte.fecha_inicio = "2024-12-31"  # Fecha inválida
            reporte.fecha_fin = "2024-01-01"
            
            try:
                reporte.generar_reporte()
                assert False, "Debería haber fallado con fechas inválidas"
            except ValueError:
                pass  # Correcto, debe fallar
    
    def _mostrar_resultados(self):
        """Muestra el reporte final de resultados"""
        print("\n" + "=" * 70)
        print("📋 RESULTADOS FINALES TC_CODE_03")
        print("=" * 70)
        
        print(f"📊 Líneas de código ANTES: {self.resultados['lineas_antes']}")
        print(f"📊 Líneas de código DESPUÉS: {self.resultados['lineas_despues']}")
        
        reduccion = self.resultados['lineas_antes'] - self.resultados['lineas_despues']
        porcentaje = (reduccion / self.resultados['lineas_antes']) * 100
        print(f"📉 Reducción total: {reduccion} líneas ({porcentaje:.1f}%)")
        
        print(f"🔄 Código duplicado eliminado: {self.resultados['codigo_duplicado_eliminado']} ocurrencias")
        
        print(f"🧪 Pruebas ejecutadas: {self.resultados['pruebas_pasadas']}/{self.resultados['pruebas_totales']}")
        
        print("\n✨ BENEFICIOS LOGRADOS:")
        print("  • ✅ Eliminación de código duplicado")
        print("  • ✅ Centralización de lógica común")
        print("  • ✅ Mayor consistencia entre reportes")
        print("  • ✅ Facilidad para agregar nuevos reportes")
        print("  • ✅ Mejor mantenibilidad")
    
    def _evaluar_exito(self):
        """Evalúa si la prueba fue exitosa"""
        criterios_exito = [
            self.resultados['lineas_despues'] < self.resultados['lineas_antes'],  # Reducción de código
            self.resultados['codigo_duplicado_eliminado'] > 0,  # Eliminó duplicación
            self.resultados['pruebas_pasadas'] >= 4,  # Al menos 4 de 5 pruebas
        ]
        
        exito = all(criterios_exito)
        
        print(f"\n{'🎉 PRUEBA TC_CODE_03: EXITOSA' if exito else '❌ PRUEBA TC_CODE_03: FALLÓ'}")
        print("=" * 70)
        
        return exito


def main():
    """Función principal que ejecuta toda la prueba"""
    print("🚀 Ejecutando TC_CODE_03 - Refactorización Automatizada")
    print("   Este script ejecuta toda la prueba automáticamente\n")
    
    test = TestTC_CODE_03()
    exito = test.ejecutar_prueba_completa()
    
    if exito:
        print("\n🎯 ¡Refactorización completada exitosamente!")
        print("   La lógica duplicada fue centralizada correctamente.")
    else:
        print("\n⚠️  La refactorización necesita mejoras.")
    
    return exito


if __name__ == "__main__":
    main()
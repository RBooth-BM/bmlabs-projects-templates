# Prompt: Generar Acción de Exportación (Excel/PDF)

Este prompt se utiliza para agregar capacidades de exportación a un servicio o vista siguiendo los estándares de BM.

## Contexto Requerido
- Entidad o endpoint de exportación.
- Formato deseado (Excel `.xlsx` o PDF).
- Si es por el lado del cliente (mapeo JSON) o servidor (Blob).

## Instrucciones para el Agente

1. **Si es Exportación Server-side (Recomendado para PDF/Reportes Pesados)**:
   - Modificar el servicio (`.service.ts`) para incluir el método de exportación.
   - Usar `responseType: 'blob'` en la configuración de Axios.
   - Implementar lógica de descarga usando `URL.createObjectURL` o el helper `downloadBlob` si existe.
   - Revocar el object URL después de usarlo.

2. **Si es Exportación Client-side (Excel ligero)**:
   - Asegurarse de tener instalada la librería `xlsx`.
   - Implementar una función que reciba el array de objetos, los mapee a etiquetas amigables y genere el workbook.
   - Utilizar la referencia local `vue-export-patterns/references/EXPORT_PATTERNS.ts` y `AppExportActions.vue` si la vista ya usa DataTable.
   - No usar `any`; tipar filtros y datos exportados.

3. **Interfaz de Usuario**:
   - Agregar un botón de descarga en el `ListHeader` o en `AppExportActions` de la tabla correspondiente.
   - Usar el ícono `Download` o `FileSpreadsheet` de `lucide-vue-next`.
   - Mostrar un estado de carga (`loading`) mientras se genera el archivo.

## Ejemplo de Prompt para el Agente
> "Agrega una función de exportación a PDF para la vista 'Ordenes de Transporte'. Modifica el `TransportOrderService` para obtener el blob desde `/TransportOrders/Export` y dispara la descarga con el nombre 'OT-FECHA.pdf'. Agrega el botón correspondiente en el header de la tabla."

## Referencias
- Skill: `vue-export-patterns`
- Referencia: `.github/skills/vue-export-patterns/references/EXPORT_PATTERNS.ts`

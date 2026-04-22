# Skill: Patrones de Exportación (Excel y PDF) - BM

Esta skill define los estándares para implementar funcionalidades de exportación de datos en Vue 3, siguiendo los patrones de producción de BM.

## 📊 Exportación a Excel (Client-side)

Para exportar datos a Excel desde el cliente, utilizamos la librería `xlsx`.

### Patrón Recomendado:
1. **Lógica de Mapeo**: Se deben traducir las llaves del JSON a labels legibles para el usuario (ej: `otNumber` -> `N° OT`).
2. **Utilidad de Descarga**: Crear un Blob con el contenido CSV/Excel y disparar la descarga mediante un link oculto.
3. **Tipado estricto**: No usar `any`; definir tipos para `params`, filas exportadas y respuesta del servicio.
4. **Reutilización**: Preferir el helper `downloadBlob` de la referencia local y el componente compartido `AppExportActions` si existe en el proyecto.

### Referencia principal:
- Ver `.github/skills/vue-export-patterns/references/EXPORT_PATTERNS.ts` para la descarga tipada.
- Ver `@/components/shared/datatable/AppExportActions.vue` si la vista ya usa DataTable y necesita acciones de exportación visibles.

## 📄 Exportación a PDF (Server-side Blob)

Para documentos como comprobantes, etiquetas o vouchers, delegamos la generación al backend y manejamos el stream como Blob.

### Patrón Recomendado:
1. **Request con Blob**: Usar `responseType: 'blob'` en la llamada de Axios.
2. **Previsualización o Impresión**: 
   - Crear URL temporal con `URL.createObjectURL(blob)`.
   - Abrir en nueva ventana o usar `window.print()` si es necesario.
   - **IMPORTANTE**: Revocar el URL después de su uso (`URL.revokeObjectURL(url)`) para evitar fugas de memoria.
3. **Contexto de ejecución**: Este patrón es browser-only. No ejecutar `window.open` ni `URL.createObjectURL` fuera de interacción cliente.

### Ejemplo de implementación (en Service):
```typescript
static async printVoucher(id: string): Promise<void> {
  const response = await httpClient.get<Blob>(`/Entity/${id}/voucher`, {
    responseType: 'blob',
  })
  const blob = new Blob([response.data], { type: 'application/pdf' })
  const url = window.URL.createObjectURL(blob)
  window.open(url, '_blank')
  window.setTimeout(() => window.URL.revokeObjectURL(url), 100)
}
```

## 🛠️ Herramientas Auxiliares
- **Composables**: Se pueden crear composables como `useExportFilename` para estandarizar los nombres de archivos (`nombre-YYYY-MM-DD.xlsx`).

## Checklist de Calidad

- [ ] ¿No hay `any` en servicios, helpers o ejemplos?
- [ ] ¿Se usa `responseType: 'blob'` para descargas server-side?
- [ ] ¿Se revoca el object URL luego de usarlo?
- [ ] ¿Se reutiliza `downloadBlob` o un helper equivalente en vez de duplicar lógica?
- [ ] ¿La acción de exportación expone `loading` en la UI?

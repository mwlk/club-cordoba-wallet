// Wrapper uniforme que devuelve el backend (Result pattern) — nunca un
// 404 para "no encontrado" en el buscador, siempre 200 con success:false.
export interface ApiResponse<T> {
  success: boolean;
  message: string | null;
  data: T | null;
}

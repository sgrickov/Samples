<script setup lang="ts">
defineProps<{ value: string | number; label: string }>()
</script>

<template>
  <div class="stat-card">
    <span class="value">{{ value }}</span>
    <span class="label">{{ label }}</span>
  </div>
</template>

<style scoped>
.stat-card {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  border: 1px solid var(--border);
  border-radius: 8px;
  padding: 0.75rem 1rem;
}

.value {
  font-size: 1.75rem;
  font-weight: 700;
}

.label {
  font-size: 0.85rem;
  color: var(--text-muted);
}
</style>

<script setup lang="ts">
defineProps<{ label?: string }>()
</script>

<template>
  <span class="new-badge"><slot>{{ label ?? 'NEW' }}</slot></span>
</template>

<style scoped>
.new-badge {
  display: inline-block;
  background: #dcfce7;
  color: #166534;
  border-radius: 999px;
  padding: 0.05rem 0.5rem;
  font-size: 0.7rem;
  font-weight: 700;
  white-space: nowrap;
}
</style>

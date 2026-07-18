<script setup lang="ts">
withDefaults(defineProps<{ rating: number; reviewCount?: number }>(), {
  reviewCount: undefined,
})
</script>

<template>
  <span class="rating-badge">
    {{ rating.toFixed(1) }}★<template v-if="reviewCount !== undefined"> ({{ reviewCount }})</template>
  </span>
</template>

<style scoped>
.rating-badge {
  white-space: nowrap;
}
</style>

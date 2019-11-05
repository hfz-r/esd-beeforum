<template>
  <div class="container">
    <ul v-if="errors" class="has-text-danger m-b-xs">
      <li v-for="(v, k) in errors" :key="k">{{ k }} {{ v[0] }}</li>
    </ul>
    <form>
      <!-- email-->
      <div class="field">
        <div class="control has-icons-left">
          <input
            :class="['input', errors['email or password'] !== undefined ? 'is-danger' : 'is-success']"
            type="email"
            placeholder="Email"
            v-model="email"
          />
          <span class="icon is-small is-left">
            <i class="fas fa-envelope"></i>
          </span>
        </div>
      </div>
      <!-- password -->
      <div class="field">
        <div class="control has-icons-left">
          <input
            :class="['input', errors['email or password'] !== undefined ? 'is-danger' : 'is-success']"
            type="password"
            placeholder="Password"
            v-model="password"
          />
          <span class="icon is-small is-left">
            <i class="fas fa-key"></i>
          </span>
        </div>
      </div>
    </form>
  </div>
</template>

<script>
import bus from "@/common/event-bus";
import { mapState } from "vuex";
import { LOGIN } from "@/store/actions.type";

export default {
  name: "Login",
  created() {
    bus.$on("MODAL_LOGIN_SUBMIT", () =>
      this.onSubmit(this.email, this.password)
    );
  },
  data() {
    return {
      email: null,
      password: null
    };
  },
  computed: {
    ...mapState({
      errors: state => state.auth.errors
    })
  },
  methods: {
    onSubmit(email, password) {
      this.$store
        .dispatch(LOGIN, { email, password })
        .then(() => this.$router.push({ name: "home" }));
    }
  }
};
</script>

<style>
</style>
<template>
  <div class="container">
    <form>
      <!-- username -->
      <div class="field">
        <label class="label">Username</label>
        <div class="control has-icons-left">
          <input
            :class="['input', errors.username !== undefined ? 'is-danger' : 'is-success']"
            type="text"
            placeholder="Enter username"
            v-model="username"
          />
          <span class="icon is-small is-left">
            <i class="fas fa-user"></i>
          </span>
        </div>
        <p
          v-show="errors.username !== undefined"
          class="help is-danger"
        >{{`Username ${errors.username}`}}</p>
      </div>
      <!-- email -->
      <div class="field">
        <label class="label">Email</label>
        <div class="control has-icons-left">
          <input
            :class="['input', errors.email !== undefined ? 'is-danger' : 'is-success']"
            type="email"
            placeholder="Enter email"
            v-model="email"
          />
          <span class="icon is-small is-left">
            <i class="fas fa-envelope"></i>
          </span>
        </div>
        <p v-show="errors.email !== undefined" class="help is-danger">{{`Email ${errors.email}`}}</p>
      </div>
      <!-- password -->
      <div class="field">
        <label class="label">Password</label>
        <div class="control has-icons-left">
          <input
            :class="['input', errors.password !== undefined ? 'is-danger' : 'is-success']"
            type="password"
            placeholder="Enter password"
            v-model="password"
          />
          <span class="icon is-small is-left">
            <i class="fas fa-key"></i>
          </span>
        </div>
        <p
          v-show="errors.password !== undefined"
          class="help is-danger"
        >{{`Password ${errors.password}`}}</p>
      </div>
    </form>
  </div>
</template>

<script>
import bus from "@/common/event-bus";
import { mapState } from "vuex";
import { REGISTER } from "@/store/actions.type";

export default {
  name: "Register",
  created() {
    bus.$on("MODAL_REGISTER_SUBMIT", () => this.onSubmit());
  },
  data() {
    return {
      username: "",
      email: "",
      password: ""
    };
  },
  computed: {
    ...mapState({
      errors: state => state.auth.errors
    })
  },
  methods: {
    onSubmit() {
      this.$store
        .dispatch(REGISTER, {
          username: this.username,
          email: this.email,
          password: this.password
        })
        .then(() => this.$router.push({ name: "home" }));
    }
  }
};
</script>

<style>
</style>
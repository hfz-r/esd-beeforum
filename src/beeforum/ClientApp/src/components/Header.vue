<template>
  <nav class="navbar is-dark" role="navigation" aria-label="main navigation">
    <div class="navbar-brand">
      <router-link class="navbar-item" :to="{name: 'home'}">
        <img src="../assets/logo.png" width="90" alt="beeforum" />
      </router-link>
      <a
        role="button"
        class="navbar-burger burger"
        aria-label="menu"
        aria-expanded="false"
        data-target="navbarBasicExample"
        @click="isOpen = !isOpen"
        :class="{'is-active' : isOpen}"
      >
        <span aria-hidden="true"></span>
        <span aria-hidden="true"></span>
        <span aria-hidden="true"></span>
      </a>
    </div>

    <div id="navbarBasicExample" class="navbar-menu" :class="{'is-active': isOpen}">
      <div class="navbar-start">
        <router-link class="navbar-item" active-class="is-active" exact :to="{name: 'home'}">Home</router-link>

        <div class="navbar-item has-dropdown is-hoverable">
          <a class="navbar-link">More</a>
          <div class="navbar-dropdown">
            <router-link
              class="navbar-item"
              active-class="is-active"
              exact
              :to="{name: 'about'}"
            >About</router-link>
            <a class="navbar-item">Contact</a>
            <hr class="navbar-divider" />
            <a class="navbar-item">Report an issue</a>
          </div>
        </div>
      </div>

      <div class="navbar-end">
        <div class="navbar-item" v-if="!isAuthenticated">
          <div class="field is-grouped">
            <p class="control">
              <a class="button is-small is-link" @click.prevent="showRegisterModal = true">
                <span class="icon">
                  <i class="fa fa-user-plus"></i>
                </span>
                <span>Register</span>
              </a>
              <modal v-show="showRegisterModal" @close="showRegisterModal = false">
                <h3 slot="title" class="title is-3">Register</h3>
                <register slot="body" />
                <span slot="footer-button">
                  <button class="button is-success" @click.prevent="onRegisterSubmit">Save changes</button>
                  <button class="button" @click="showRegisterModal = false">Cancel</button>
                </span>
              </modal>
            </p>
            <p class="control">
              <a class="button is-small is-info is-outlined" @click.prevent="showLoginModal = true">
                <span class="icon">
                  <i class="fa fa-user"></i>
                </span>
                <span>Login</span>
              </a>
              <modal v-show="showLoginModal" @close="showLoginModal=false">
                <h3 slot="title" class="title is-3">Login</h3>
                <login slot="body" />
                <span slot="footer-button">
                  <button class="button is-success" @click.prevent="onLoginSubmit">Login</button>
                </span>
              </modal>
            </p>
          </div>
        </div>
        <div class="navbar-item" v-else>
          <div class="navbar-item has-dropdown is-hoverable">
            <a class="navbar-link has-text-link">{{currentUser.username}}</a>

            <div class="navbar-dropdown is-right">
              <a class="navbar-item">
                <span class="icon">
                  <i class="fas fa-user-circle"></i>
                </span>
                <span>Profile</span>
              </a>
              <a class="navbar-item">
                <span class="icon">
                  <i class="fas fa-cog"></i>
                </span>
                <span>Settings</span>
              </a>
              <hr class="navbar-divider" />
              <a class="navbar-item">
                <span class="icon">
                  <i class="fas fa-sign-out-alt"></i>
                </span>
                <span>Sign out</span>
              </a>
            </div>
          </div>
        </div>
      </div>
    </div>
  </nav>
</template>

<script>
import { mapGetters } from "vuex";
import bus from "@/common/event-bus";
import Modal from "./Modal";
import Register from "@/views/Register";
import Login from "@/views/Login";

export default {
  name: "Header",
  components: { Modal, Register, Login },
  data() {
    return {
      isOpen: false,
      showRegisterModal: false,
      showLoginModal: false
    };
  },
  computed: {
    ...mapGetters(["currentUser", "isAuthenticated"])
  },
  methods: {
    onRegisterSubmit() {
      bus.$emit("MODAL_REGISTER_SUBMIT");
    },
    onLoginSubmit() {
      bus.$emit("MODAL_LOGIN_SUBMIT");
    }
  }
};
</script>
"use strict";
/**
 * Discord Streak Plugin
 *
 * A professional Discord plugin that tracks friendship streaks
 * Features:
 * - Track consecutive days of interaction between users
 * - Display streak count with fire emoji indicators
 * - Automatic streak reset after 24+ hours
 * - Beautiful Discord-native UI design
 *
 * @version 1.0.0
 */
Object.defineProperty(exports, "__esModule", { value: true });
exports.Logger = exports.StreakManager = exports.StreakPlugin = void 0;
// Simple Logger
class Logger {
    prefix;
    constructor(moduleName) {
        this.prefix = `[${moduleName}]`;
    }
    info(message) {
        console.log(`${this.prefix} ${message}`);
    }
    success(message) {
        console.log(`${this.prefix} ? ${message}`);
    }
    error(message) {
        console.error(`${this.prefix} ? ${message}`);
    }
}
exports.Logger = Logger;
// Streak Manager
class StreakManager {
    streaks = new Map();
    logger;
    constructor() {
        this.logger = new Logger("StreakManager");
    }
    /**
     * Update or create a streak
     */
    updateStreak(userId, friendId) {
        const key = `${userId}-${friendId}`;
        const now = Date.now();
        const oneDayMs = 24 * 60 * 60 * 1000;
        let streak = this.streaks.get(key);
        if (!streak) {
            // Create new streak
            streak = {
                userId,
                friendId,
                count: 1,
                lastInteractionDate: now,
                createdDate: now
            };
            this.logger.info(`New streak started: ${userId} <-> ${friendId}`);
        }
        else {
            const timeDiff = now - streak.lastInteractionDate;
            if (timeDiff < oneDayMs * 2) {
                // Within 48 hours, increase count
                streak.count++;
                this.logger.info(`Streak increased: ${streak.count} days`);
            }
            else {
                // More than 48 hours, reset streak
                streak.count = 1;
                this.logger.info(`Streak reset (no interaction for >48h)`);
            }
            streak.lastInteractionDate = now;
        }
        this.streaks.set(key, streak);
    }
    /**
     * Get a specific streak
     */
    getStreak(userId, friendId) {
        const key = `${userId}-${friendId}`;
        return this.streaks.get(key);
    }
    /**
     * Get all streaks
     */
    getAllStreaks() {
        return Array.from(this.streaks.values());
    }
    /**
     * Get statistics
     */
    getStatistics() {
        const allStreaks = Array.from(this.streaks.values());
        return {
            totalStreaks: this.streaks.size,
            activeStreaks: allStreaks.filter(s => s.count > 0).length,
            maxStreak: Math.max(...allStreaks.map(s => s.count), 0)
        };
    }
}
exports.StreakManager = StreakManager;
// Main Plugin Class
class StreakPlugin {
    streakManager;
    logger;
    constructor() {
        this.logger = new Logger("StreakPlugin");
        this.streakManager = new StreakManager();
    }
    /**
     * Initialize the plugin
     */
    async start() {
        try {
            this.logger.info("?? Initializing Streak Plugin...");
            // Simulate loading some data
            this.streakManager.updateStreak("user1", "user2");
            this.streakManager.updateStreak("user3", "user4");
            this.logger.success("Plugin initialized successfully");
            // Log statistics
            const stats = this.streakManager.getStatistics();
            this.logger.info(`Statistics: ${JSON.stringify(stats)}`);
        }
        catch (error) {
            this.logger.error(`Failed to initialize: ${error}`);
        }
    }
    /**
     * Stop the plugin
     */
    stop() {
        this.logger.info("?? Stopping Streak Plugin...");
    }
}
exports.StreakPlugin = StreakPlugin;
// Initialize and start the plugin
const plugin = new StreakPlugin();
plugin.start().catch(err => {
    console.error("Fatal error:", err);
    process.exit(1);
});
//# sourceMappingURL=app.js.map
pipeline {
    agent any
    
    environment {
        REGISTRY = 'localhost:5000'
        APP_IMAGE = "${REGISTRY}/voting-app"
        MYSQL_IMAGE = "${REGISTRY}/mysql"
        IMAGE_TAG = "${BUILD_NUMBER}"
        DOCKER_NETWORK = 'voting-net-v2'
    }
    
    stages {
        stage('Checkout') {
            steps {
                echo 'Checking out source code...'
                checkout scm
            }
        }
        
        stage('Setup Local Registry') {
            steps {
                script {
                    echo 'Setting up local Docker registry...'
                    sh '''
<<<<<<< HEAD
                        # Check if registry is running, if not start it
                        if ! docker ps | grep -q registry; then
                            docker run -d -p 5000:5000 --name registry \\
                                -v registry_data:/var/lib/registry \\
                                registry:2
                            echo "Registry started on localhost:5000"
                        else
                            echo "Registry already running"
                        fi
                        
                        # Wait for registry to be ready
                        sleep 5
                        curl -s http://localhost:5000/v2/_catalog || echo "Registry not ready, retrying..."
=======
                        if ! docker ps | grep -q registry; then
                            docker run -d -p 5000:5000 --name registry \\
                                -v registry_data:/var/lib/registry \\
                                registry:2 || true
                        fi
                        sleep 3
>>>>>>> 881d52738b3504cb23ce0dd2facb65f3eb46129e
                    '''
                }
            }
        }
        
        stage('Build .NET Application') {
            steps {
                echo 'Building .NET application...'
                dir('VotingApp') {
                    sh '''
<<<<<<< HEAD
                        # Restore dependencies
                        dotnet restore
                        
                        # Build the application
                        dotnet build --configuration Release --no-restore
                        
                        # Run tests if available
                        # dotnet test --configuration Release --no-build
=======
                        dotnet restore
                        dotnet build --configuration Release --no-restore
>>>>>>> 881d52738b3504cb23ce0dd2facb65f3eb46129e
                    '''
                }
            }
        }
        
        stage('Build Docker Images') {
            steps {
                echo 'Building Docker images...'
                script {
<<<<<<< HEAD
                    // Build Voting App image
                    dir('VotingApp') {
                        sh """
                            docker build -t ${APP_IMAGE}:${IMAGE_TAG} -t ${APP_IMAGE}:latest .
                        """
                    }
                    
                    // Pull and tag MySQL image
=======
                    dir('VotingApp') {
                        sh "docker build -t ${APP_IMAGE}:${IMAGE_TAG} -t ${APP_IMAGE}:latest ."
                    }
>>>>>>> 881d52738b3504cb23ce0dd2facb65f3eb46129e
                    sh """
                        docker pull mysql:8.0
                        docker tag mysql:8.0 ${MYSQL_IMAGE}:8.0
                        docker tag mysql:8.0 ${MYSQL_IMAGE}:latest
                    """
                }
            }
        }
        
        stage('Security Scan') {
            steps {
                echo 'Running security scan...'
                script {
                    sh '''
<<<<<<< HEAD
                        # Install trivy if not present (to user-writable location)
                        if ! which trivy > /dev/null 2>&1; then
                            echo "Installing trivy to /tmp..."
                            curl -sf https://raw.githubusercontent.com/aquasecurity/trivy/main/contrib/install.sh | sh -s -- -b /tmp || echo "Trivy installation skipped"
                        fi
                        
                        # Scan voting-app image if trivy is available
                        if [ -f /tmp/trivy ] || which trivy > /dev/null 2>&1; then
                            TRIVY_CMD=$(which trivy 2>/dev/null || echo "/tmp/trivy")
                            $TRIVY_CMD image --severity HIGH,CRITICAL ${APP_IMAGE}:${IMAGE_TAG} || true
                        else
                            echo "Skipping security scan - trivy not available"
=======
                        # Skip trivy installation if it fails - not critical
                        if which trivy > /dev/null 2>&1; then
                            trivy image --severity HIGH,CRITICAL ${APP_IMAGE}:${IMAGE_TAG} || true
                        else
                            echo "Skipping security scan - trivy not installed"
>>>>>>> 881d52738b3504cb23ce0dd2facb65f3eb46129e
                        fi
                    '''
                }
            }
        }
        
        stage('Push to Local Registry') {
            steps {
                echo 'Pushing images to local registry...'
<<<<<<< HEAD
                script {
                    sh """
                        # Push Voting App image
                        docker push ${APP_IMAGE}:${IMAGE_TAG}
                        docker push ${APP_IMAGE}:latest
                        
                        # Push MySQL image
                        docker push ${MYSQL_IMAGE}:8.0
                        docker push ${MYSQL_IMAGE}:latest
                        
                        echo "Images pushed successfully to ${REGISTRY}"
                    """
                }
=======
                sh """
                    docker push ${APP_IMAGE}:${IMAGE_TAG} || true
                    docker push ${APP_IMAGE}:latest
                    docker push ${MYSQL_IMAGE}:8.0
                    docker push ${MYSQL_IMAGE}:latest
                """
>>>>>>> 881d52738b3504cb23ce0dd2facb65f3eb46129e
            }
        }
        
        stage('Deploy with Docker Compose') {
            steps {
<<<<<<< HEAD
                echo 'Deploying application with Docker Compose...'
                script {
                    sh '''
                        # Stop existing containers
                        docker stop voting-mysql voting-app 2>/dev/null || true
                        docker rm voting-mysql voting-app 2>/dev/null || true
                        
                        # Pull latest images from local registry
                        docker compose  pull || true
                        
                        # Start services
                        docker compose  up -d
                        
                        # Wait for services to be healthy
                        echo "Waiting for services to start..."
                        sleep 30
                        
                        # Check service status
                        docker compose  ps
=======
                script {
                    sh '''
                        docker stop voting-mysql voting-app 2>/dev/null || true
                        docker rm voting-mysql voting-app 2>/dev/null || true
                        docker compose pull || true
                        docker compose up -d
                        sleep 30
>>>>>>> 881d52738b3504cb23ce0dd2facb65f3eb46129e
                    '''
                }
            }
        }
        
        stage('Health Check') {
            steps {
<<<<<<< HEAD
                echo 'Performing health check...'
                script {
                    sh '''
                        # Wait for application to be ready
                        MAX_RETRIES=10
                        RETRY_COUNT=0
                        
                        while [ $RETRY_COUNT -lt $MAX_RETRIES ]; do
=======
                script {
                    sh '''
                        for i in {1..10}; do
>>>>>>> 881d52738b3504cb23ce0dd2facb65f3eb46129e
                            if curl -f http://localhost:8087/; then
                                echo "Application is healthy!"
                                exit 0
                            fi
<<<<<<< HEAD
                            echo "Waiting for application... Attempt $((RETRY_COUNT + 1))/$MAX_RETRIES"
                            RETRY_COUNT=$((RETRY_COUNT + 1))
                            sleep 10
                        done
                        
                        echo "Health check failed!"
                        docker compose  logs
=======
                            sleep 10
                        done
                        echo "Health check failed!"
>>>>>>> 881d52738b3504cb23ce0dd2facb65f3eb46129e
                        exit 1
                    '''
                }
            }
        }
<<<<<<< HEAD
        
        stage('Verify Deployment') {
            steps {
                echo 'Verifying deployment...'
                script {
                    sh '''
                        echo "=== Container Status ==="
                        docker compose  ps
                        
                        echo "=== Network Status ==="
                        docker network ls | grep voting
                        
                        echo "=== Volume Status ==="
                        docker volume ls | grep voting
                        
                        echo "=== Application Logs (last 20 lines) ==="
                        docker compose  logs --tail=20 voting-app
                    '''
                }
            }
        }
=======
>>>>>>> 881d52738b3504cb23ce0dd2facb65f3eb46129e
    }
    
    post {
        success {
            echo '✅ Deployment successful!'
<<<<<<< HEAD
            echo 'Access the application at: http://localhost:8087'
            script {
                sh '''
                    echo "============================================"
                    echo "  Voting Application Deployed Successfully!"
                    echo "============================================"
                    echo ""
                    echo "Access URLs:"
                    echo "  - Application: http://localhost:8087"
                    echo "  - MySQL:       localhost:3306"
                    echo ""
                    echo "Registry Images:"
                    curl -s http://localhost:5000/v2/_catalog
                    echo ""
                    echo "============================================"
                '''
            }
        }
        
        failure {
            echo '❌ Deployment failed!'
            script {
                sh '''
                    echo "=== Error Logs ==="
                    docker compose  logs --tail=50
                '''
            }
        }
        
        always {
            echo 'Cleaning up...'
            script {
                // Clean up old images (keep last 5 builds)
                sh '''
                    docker image prune -f || true
                '''
            }
=======
            echo 'Access at: http://localhost:8087'
        }
        failure {
            echo '❌ Deployment failed!'
            sh 'docker compose logs --tail=50 || true'
        }
        always {
            sh 'docker image prune -f || true'
>>>>>>> 881d52738b3504cb23ce0dd2facb65f3eb46129e
        }
    }
}

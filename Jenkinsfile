pipeline {
    agent any
    
    environment {
        REGISTRY = 'localhost:5000'
        APP_IMAGE = "${REGISTRY}/voting-app"
        MYSQL_IMAGE = "${REGISTRY}/mysql"
        IMAGE_TAG = "${BUILD_NUMBER}"
    }
    
    stages {
        stage('Cleanup Infrastructure') {
            steps {
                echo '🧹 Wiping old environment...'
                script {
                    sh '''
                        docker stop voting-app voting-mysql registry 2>/dev/null || true
                        docker rm voting-app voting-mysql registry 2>/dev/null || true
                        docker volume rm voting_db_data_v2 2>/dev/null || true
                        docker network rm voting-net-v2 2>/dev/null || true
                    '''
                }
            }
        }
        
        stage('Setup Local Registry') {
            steps {
                sh 'docker run -d -p 5000:5000 --name registry --restart unless-stopped registry:2 2>/dev/null || true'
                sleep 5
            }
        }
        
        stage('Checkout & Build') {
            steps {
                checkout scm
                dir('VotingApp') {
                    sh 'dotnet restore'
                    sh 'dotnet build --configuration Release --no-restore'
                }
            }
        }
        
        stage('Build & Push Images') {
            steps {
                script {
                    dir('VotingApp') {
                        sh "docker build -t ${APP_IMAGE}:latest -t ${APP_IMAGE}:${IMAGE_TAG} ."
                    }
                    sh """
                        docker pull mysql:8.0
                        docker tag mysql:8.0 ${MYSQL_IMAGE}:8.0
                        docker push ${APP_IMAGE}:latest
                        docker push ${MYSQL_IMAGE}:8.0
                    """
                }
            }
        }
        
        stage('Deploy') {
            steps {
                script {
                    sh '''
                        docker compose pull
                        docker compose up -d
                        echo "⏳ Waiting for migrations..."
                        sleep 45
                    '''
                }
            }
        }
        
        stage('Wait for Voting App') {
            steps {
                script {
                    sh '''
                        echo "Checking App (Waiting for Migrations to finish)..."
                        # Use standard sequence for better compatibility
                        for i in $(seq 1 30); do
                            HTTP_CODE=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:8087/ || echo "000")
                            if [ "$HTTP_CODE" = "200" ]; then
                                echo "✅ SUCCESS: App is UP!"
                                exit 0
                            fi
                            echo "Status $HTTP_CODE... Migration in progress... (Try $i/30)"
                            sleep 10
                        done
                        docker compose logs voting-app
                        exit 1
                    '''
                }
            }
        }
    }
    
    post {
        success {
            echo '🚀 DEPLOYMENT SUCCESSFUL! Access at http://localhost:8087'
        }
        failure {
            echo '❌ DEPLOYMENT FAILED!'
            sh 'docker compose logs --tail=100 || true'
        }
    }
}
